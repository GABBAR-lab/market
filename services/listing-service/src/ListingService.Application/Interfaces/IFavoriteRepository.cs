namespace ListingService.Application.Interfaces;

public interface IFavoriteRepository
{
    Task<IReadOnlyList<Guid>> GetListingIdsByUserAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, Guid listingId);
    Task AddAsync(Guid userId, Guid listingId);
    Task RemoveAsync(Guid userId, Guid listingId);
}
