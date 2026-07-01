using ChatService.Application.Common;
using ChatService.Application.DTOs;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MarketPlace.Shared.Contracts.Events;
using MarketPlace.Shared.Messaging;

namespace ChatService.Application.Services;

public class ChatAppService : IChatAppService
{
    private readonly IChatRepository _repository;
    private readonly IEventPublisher _eventPublisher;

    public ChatAppService(IChatRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<ConversationResponse>> StartOrGetConversationAsync(
        Guid buyerId, StartConversationRequest request, CancellationToken cancellationToken = default)
    {
        if (buyerId == request.SellerId)
        {
            return Result<ConversationResponse>.Failure("Cannot start a conversation with yourself.");
        }

        var existing = await _repository.FindConversationAsync(request.ListingId, buyerId, request.SellerId, cancellationToken);
        if (existing is not null)
        {
            if (!string.IsNullOrWhiteSpace(request.InitialMessage))
            {
                await SendMessageInternalAsync(existing, buyerId, request.InitialMessage, cancellationToken);
            }

            return Result<ConversationResponse>.Success(await MapConversationAsync(existing, buyerId, cancellationToken));
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ListingId = request.ListingId,
            BuyerId = buyerId,
            SellerId = request.SellerId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateConversationAsync(conversation, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.InitialMessage))
        {
            await SendMessageInternalAsync(conversation, buyerId, request.InitialMessage, cancellationToken);
        }

        return Result<ConversationResponse>.Success(await MapConversationAsync(conversation, buyerId, cancellationToken));
    }

    public async Task<Result<IReadOnlyList<ConversationResponse>>> GetMyConversationsAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var conversations = await _repository.GetUserConversationsAsync(userId, cancellationToken);
        var responses = new List<ConversationResponse>();
        foreach (var conversation in conversations)
        {
            responses.Add(await MapConversationAsync(conversation, userId, cancellationToken));
        }

        return Result<IReadOnlyList<ConversationResponse>>.Success(responses);
    }

    public async Task<Result<IReadOnlyList<MessageResponse>>> GetMessagesAsync(
        Guid userId, Guid conversationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var conversation = await _repository.GetConversationAsync(conversationId, cancellationToken);
        if (conversation is null || !IsParticipant(conversation, userId))
        {
            return Result<IReadOnlyList<MessageResponse>>.Failure("Conversation not found.");
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var messages = await _repository.GetMessagesAsync(conversationId, page, pageSize, cancellationToken);
        await _repository.MarkMessagesReadAsync(conversationId, userId, cancellationToken);

        return Result<IReadOnlyList<MessageResponse>>.Success(messages.Select(MapMessage).ToList());
    }

    public async Task<Result<MessageResponse>> SendMessageAsync(
        Guid userId, Guid conversationId, SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<MessageResponse>.Failure("Message cannot be empty.");
        }

        var conversation = await _repository.GetConversationAsync(conversationId, cancellationToken);
        if (conversation is null || !IsParticipant(conversation, userId))
        {
            return Result<MessageResponse>.Failure("Conversation not found.");
        }

        var message = await SendMessageInternalAsync(conversation, userId, request.Content.Trim(), cancellationToken);
        return Result<MessageResponse>.Success(MapMessage(message));
    }

    private async Task<ChatMessage> SendMessageInternalAsync(
        Conversation conversation, Guid senderId, string content, CancellationToken cancellationToken)
    {
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = senderId,
            Content = content,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _repository.AddMessageAsync(message, cancellationToken);

        var recipientId = senderId == conversation.BuyerId ? conversation.SellerId : conversation.BuyerId;
        await _eventPublisher.PublishAsync(EventTypes.ChatMessageSent, new ChatMessagePayload(
            conversation.Id, senderId, recipientId, content, message.SentAt), cancellationToken);

        await _eventPublisher.PublishAsync(EventTypes.NotificationSend, new NotificationSendPayload(
            recipientId, "New message", content.Length > 80 ? content[..80] + "…" : content,
            "InApp", "Chat", conversation.Id), cancellationToken);

        return message;
    }

    private async Task<ConversationResponse> MapConversationAsync(
        Conversation conversation, Guid userId, CancellationToken cancellationToken)
    {
        var messages = await _repository.GetMessagesAsync(conversation.Id, 1, 1, cancellationToken);
        var last = messages.FirstOrDefault();
        var unread = await _repository.GetUnreadCountAsync(conversation.Id, userId, cancellationToken);

        return new ConversationResponse(
            conversation.Id,
            conversation.ListingId,
            conversation.BuyerId,
            conversation.SellerId,
            last?.Content,
            last?.SentAt ?? conversation.LastMessageAt,
            unread);
    }

    private static bool IsParticipant(Conversation conversation, Guid userId) =>
        conversation.BuyerId == userId || conversation.SellerId == userId;

    private static MessageResponse MapMessage(ChatMessage message) => new(
        message.Id, message.ConversationId, message.SenderId, message.Content, message.IsRead, message.SentAt);
}
