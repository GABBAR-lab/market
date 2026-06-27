namespace ListingService.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Guid> CreatePaymentAsync(
        Guid listingId,
        Guid sellerId,
        decimal amount,
        int durationDays,
        decimal perDayPrice,
        string paymentMethod,
        string transactionRef,
        string cardLastFour);

    Task MarkCompletedAsync(Guid paymentId);
}

public interface IAppSettingsRepository
{
    Task<int> GetIntAsync(string key, int defaultValue);
    Task<bool> GetBoolAsync(string key, bool defaultValue);
}
