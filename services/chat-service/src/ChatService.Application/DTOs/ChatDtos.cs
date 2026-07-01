namespace ChatService.Application.DTOs;

public record ConversationResponse(
    Guid Id,
    Guid ListingId,
    Guid BuyerId,
    Guid SellerId,
    string? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount);

public record MessageResponse(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    bool IsRead,
    DateTime SentAt);

public record StartConversationRequest(Guid ListingId, Guid SellerId, string? InitialMessage);
public record SendMessageRequest(string Content);
