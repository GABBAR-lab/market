using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Persistence;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> Messages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(e =>
        {
            e.ToTable("Conversations");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.ListingId, x.BuyerId, x.SellerId }).IsUnique();
            e.HasIndex(x => x.BuyerId);
            e.HasIndex(x => x.SellerId);
        });

        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.ToTable("Messages");
            e.HasKey(x => x.Id);
            e.Property(x => x.Content).HasMaxLength(4000);
            e.HasIndex(x => x.ConversationId);
            e.HasOne(x => x.Conversation).WithMany(c => c.Messages).HasForeignKey(x => x.ConversationId);
        });
    }
}

public class ChatRepository : IChatRepository
{
    private readonly ChatDbContext _db;

    public ChatRepository(ChatDbContext db) => _db = db;

    public async Task<Conversation?> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default) =>
        await _db.Conversations.FindAsync([conversationId], cancellationToken);

    public async Task<Conversation?> FindConversationAsync(
        Guid listingId, Guid buyerId, Guid sellerId, CancellationToken cancellationToken = default) =>
        await _db.Conversations.FirstOrDefaultAsync(
            c => c.ListingId == listingId && c.BuyerId == buyerId && c.SellerId == sellerId, cancellationToken);

    public async Task<IReadOnlyList<Conversation>> GetUserConversationsAsync(
        Guid userId, CancellationToken cancellationToken = default) =>
        await _db.Conversations
            .Where(c => c.BuyerId == userId || c.SellerId == userId)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Conversation> CreateConversationAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _db.Conversations.Add(conversation);
        await _db.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    public async Task<ChatMessage> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        _db.Messages.Add(message);
        var conversation = await _db.Conversations.FindAsync([message.ConversationId], cancellationToken);
        if (conversation is not null)
        {
            conversation.LastMessageAt = message.SentAt;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(
        Guid conversationId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        await _db.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);

    public async Task MarkMessagesReadAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var unread = await _db.Messages
            .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var message in unread)
        {
            message.IsRead = true;
        }

        if (unread.Count > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default) =>
        await _db.Messages.CountAsync(
            m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead, cancellationToken);
}
