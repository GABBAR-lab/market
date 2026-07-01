namespace LoggingService.Domain.Entities;

public class ServiceLogEntry
{
    public Guid Id { get; set; }
    public string Level { get; set; } = "Information";
    public string Service { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? Exception { get; set; }
    public string? PayloadJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
