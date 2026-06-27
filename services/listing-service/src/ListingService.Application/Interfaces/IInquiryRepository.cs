namespace ListingService.Application.Interfaces;

public interface IInquiryRepository
{
    Task<Guid> CreateAsync(Guid listingId, Guid? buyerUserId, string buyerName, string buyerPhone, string message);
    Task<int> CountByListingAsync(Guid listingId);
    Task<IReadOnlyList<InquiryRecord>> GetBySellerAsync(Guid sellerId);
}

public record InquiryRecord(
    Guid Id,
    Guid ListingId,
    string ListingTitle,
    string BuyerName,
    string BuyerPhone,
    string Message,
    string Status,
    DateTime CreatedAt);
