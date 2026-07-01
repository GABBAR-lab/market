namespace MarketPlace.Shared.Contracts.Events;

public static class EventTypes
{
    public const string PaymentCompleted = "payment.completed";
    public const string PaymentFailed = "payment.failed";
    public const string ListingCreated = "listing.created";
    public const string ListingActivated = "listing.activated";
    public const string NotificationSend = "notification.send";
    public const string ChatMessageSent = "chat.message.sent";
    public const string LogEntry = "logging.entry";
}

public record IntegrationEvent(
    string EventType,
    Guid EventId,
    DateTime OccurredAt,
    string PayloadJson);

public record PaymentCompletedPayload(
    Guid PaymentId,
    Guid ListingId,
    Guid SellerId,
    decimal Amount,
    string Currency,
    string TransactionRef,
    string ListingStatus);

public record NotificationSendPayload(
    Guid UserId,
    string Title,
    string Body,
    string Channel,
    string? ReferenceType,
    Guid? ReferenceId);

public record ChatMessagePayload(
    Guid ConversationId,
    Guid SenderId,
    Guid RecipientId,
    string Message,
    DateTime SentAt);

public record LogEntryPayload(
    string Level,
    string Service,
    string Message,
    string? CorrelationId,
    string? UserId,
    string? Exception);
