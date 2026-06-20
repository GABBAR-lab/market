using ListingService.Application.DTOs.Listings;
using ListingService.Domain.Entities;

namespace ListingService.Application.Interfaces;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(Guid id, bool includeDetails = false);
    Task<Listing?> GetBySlugAsync(string slug, bool includeDetails = false);
    Task<(IReadOnlyList<Listing> Items, int TotalCount)> SearchAsync(ListingSearchRequest request);
    Task<IReadOnlyList<Listing>> GetFeaturedAsync(int limit);
    Task<IReadOnlyList<Listing>> GetBySellerIdAsync(Guid sellerId);
    Task AddAsync(Listing listing);
    Task UpdateAsync(Listing listing);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
}
