namespace NotificationService.Application.DTOs;

public record NotificationResponse(
    Guid Id,
    string Title,
    string Body,
    string Channel,
    string? ReferenceType,
    Guid? ReferenceId,
    bool IsRead,
    DateTime CreatedAt);

public record UnreadCountResponse(int Count);

public record CreateNotificationRequest(
    Guid UserId,
    string Title,
    string Body,
    string Channel,
    string? ReferenceType,
    Guid? ReferenceId);
