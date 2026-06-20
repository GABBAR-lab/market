using ListingService.Application.Common;
using ListingService.Application.DTOs.Listings;

namespace ListingService.Application.Interfaces;

public interface IListingService
{
    Task<Result<ListingDetailResponse>> CreateAsync(Guid sellerId, CreateListingRequest request);
    Task<Result<ListingDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<ListingDetailResponse>> GetBySlugAsync(string slug);
    Task<Result<PagedResult<ListingResponse>>> SearchAsync(ListingSearchRequest request);
    Task<Result<IReadOnlyList<ListingResponse>>> GetFeaturedAsync(int limit = 10);
    Task<Result<IReadOnlyList<ListingResponse>>> GetMyListingsAsync(Guid sellerId);
    Task<Result<ListingDetailResponse>> UpdateAsync(Guid id, Guid sellerId, bool isAdmin, UpdateListingRequest request);
    Task<Result<bool>> DeleteAsync(Guid id, Guid sellerId, bool isAdmin);
    Task<Result<ListingDetailResponse>> SubmitForReviewAsync(Guid id, Guid sellerId, bool isAdmin);
    Task<Result<ListingDetailResponse>> PublishAsync(Guid id);
    Task<Result<ListingDetailResponse>> RejectAsync(Guid id);
    Task<Result<ListingDetailResponse>> MarkAsSoldAsync(Guid id, Guid sellerId, bool isAdmin);
    Task<Result<ListingDetailResponse>> FeatureAsync(Guid id, FeatureListingRequest request);
    Task<Result<ListingDetailResponse>> RemoveFeaturedAsync(Guid id);
    Task<Result<ListingImageResponse>> AddImageAsync(Guid listingId, Guid sellerId, bool isAdmin, AddListingImageRequest request);
    Task<Result<bool>> RemoveImageAsync(Guid listingId, Guid imageId, Guid sellerId, bool isAdmin);
    Task<Result<ListingDetailResponse>> IncrementViewCountAsync(Guid id);
}
