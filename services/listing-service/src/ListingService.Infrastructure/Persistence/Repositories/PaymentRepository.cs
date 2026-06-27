using Dapper;
using ListingService.Application.Interfaces;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public PaymentRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreatePaymentAsync(
        Guid listingId,
        Guid sellerId,
        decimal amount,
        int durationDays,
        decimal perDayPrice,
        string paymentMethod,
        string transactionRef,
        string cardLastFour)
    {
        using var connection = _connectionFactory.CreateConnection();
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await connection.ExecuteAsync(@"
            INSERT INTO ListingPayments (
                Id, ListingId, SellerId, Amount, Currency, DurationDays, PerDayPrice,
                Status, PaymentMethod, TransactionRef, CardLastFour, PaidAt, CreatedAt)
            VALUES (
                @Id, @ListingId, @SellerId, @Amount, 'LKR', @DurationDays, @PerDayPrice,
                'Completed', @PaymentMethod, @TransactionRef, @CardLastFour, @PaidAt, @CreatedAt)",
            new
            {
                Id = id,
                ListingId = listingId,
                SellerId = sellerId,
                Amount = amount,
                DurationDays = durationDays,
                PerDayPrice = perDayPrice,
                PaymentMethod = paymentMethod,
                TransactionRef = transactionRef,
                CardLastFour = cardLastFour,
                PaidAt = now,
                CreatedAt = now
            });

        return id;
    }

    public async Task MarkCompletedAsync(Guid paymentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE ListingPayments SET Status = 'Completed', UpdatedAt = @Now WHERE Id = @Id",
            new { Id = paymentId, Now = DateTime.UtcNow });
    }
}

public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AppSettingsRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> GetIntAsync(string key, int defaultValue)
    {
        using var connection = _connectionFactory.CreateConnection();
        var value = await connection.ExecuteScalarAsync<string?>(
            "SELECT [Value] FROM AppSettings WHERE [Key] = @Key", new { Key = key });

        return int.TryParse(value, out var parsed) ? parsed : defaultValue;
    }

    public async Task<bool> GetBoolAsync(string key, bool defaultValue)
    {
        using var connection = _connectionFactory.CreateConnection();
        var value = await connection.ExecuteScalarAsync<string?>(
            "SELECT [Value] FROM AppSettings WHERE [Key] = @Key", new { Key = key });

        return bool.TryParse(value, out var parsed) ? parsed : defaultValue;
    }
}
