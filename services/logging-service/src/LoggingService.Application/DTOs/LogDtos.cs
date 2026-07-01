namespace LoggingService.Application.DTOs;

public record LogEntryResponse(
    Guid Id,
    string Level,
    string Service,
    string EventType,
    string Message,
    string? CorrelationId,
    string? UserId,
    string? Exception,
    DateTime CreatedAt);

public record LogSearchRequest(
    string? Service = null,
    string? Level = null,
    string? EventType = null,
    DateTime? From = null,
    DateTime? To = null,
    int Page = 1,
    int PageSize = 50);
