namespace ListingService.Application.Interfaces;

public interface IReportRepository
{
    Task<Guid> CreateAsync(Guid listingId, Guid? reporterUserId, string reason, string? comment);
}
