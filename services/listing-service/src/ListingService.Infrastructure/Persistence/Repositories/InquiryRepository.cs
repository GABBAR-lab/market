using Dapper;
using ListingService.Application.Interfaces;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class InquiryRepository : IInquiryRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public InquiryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(Guid listingId, Guid? buyerUserId, string buyerName, string buyerPhone, string message)
    {
        var id = Guid.NewGuid();
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            @"INSERT INTO ListingInquiries (Id, ListingId, BuyerUserId, BuyerName, BuyerPhone, Message, Status, CreatedAt)
              VALUES (@Id, @ListingId, @BuyerUserId, @BuyerName, @BuyerPhone, @Message, 'New', @CreatedAt)",
            new
            {
                Id = id,
                ListingId = listingId,
                BuyerUserId = buyerUserId,
                BuyerName = buyerName,
                BuyerPhone = buyerPhone,
                Message = message,
                CreatedAt = DateTime.UtcNow
            });
        return id;
    }

    public async Task<int> CountByListingAsync(Guid listingId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM ListingInquiries WHERE ListingId = @ListingId",
            new { ListingId = listingId });
    }

    public async Task<IReadOnlyList<InquiryRecord>> GetBySellerAsync(Guid sellerId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<InquiryRecord>(
            @"SELECT i.Id, i.ListingId, l.Title AS ListingTitle, i.BuyerName, i.BuyerPhone, i.Message, i.Status, i.CreatedAt
              FROM ListingInquiries i
              INNER JOIN Listings l ON l.Id = i.ListingId
              WHERE l.SellerId = @SellerId
              ORDER BY i.CreatedAt DESC",
            new { SellerId = sellerId });
        return rows.ToList();
    }
}
