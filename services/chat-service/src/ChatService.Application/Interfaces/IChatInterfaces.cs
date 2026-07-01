using ChatService.Application.Common;
using ChatService.Application.DTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces;

public interface IChatRepository
{
    Task<Conversation?> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Conversation?> FindConversationAsync(Guid listingId, Guid buyerId, Guid sellerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Conversation> CreateConversationAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task<ChatMessage> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(Guid conversationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task MarkMessagesReadAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);
}

public interface IChatAppService
{
    Task<Result<ConversationResponse>> StartOrGetConversationAsync(Guid buyerId, StartConversationRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ConversationResponse>>> GetMyConversationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<MessageResponse>>> GetMessagesAsync(Guid userId, Guid conversationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<MessageResponse>> SendMessageAsync(Guid userId, Guid conversationId, SendMessageRequest request, CancellationToken cancellationToken = default);
}
