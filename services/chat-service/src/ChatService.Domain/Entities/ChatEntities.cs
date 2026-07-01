namespace ChatService.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public Conversation? Conversation { get; set; }
}
