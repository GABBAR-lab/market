using ListingService.Application.Common;
using ListingService.Application.DTOs.Listings;
using ListingService.Application.Interfaces;
using ListingService.Domain.Entities;
using ListingService.Domain.Enums;

namespace ListingService.Application.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _listingRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IAppSettingsRepository _appSettingsRepository;

    public ListingService(
        IListingRepository listingRepository,
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository,
        IAppSettingsRepository appSettingsRepository)
    {
        _listingRepository = listingRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
        _appSettingsRepository = appSettingsRepository;
    }

    public async Task<Result<ListingDetailResponse>> CreateAsync(Guid sellerId, CreateListingRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, includeAttributes: true);
        if (category is null || !category.IsActive)
        {
            return Result<ListingDetailResponse>.Failure("Category not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 150)
        {
            return Result<ListingDetailResponse>.Failure("Title is required and must be at most 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.ListingPurpose))
        {
            return Result<ListingDetailResponse>.Failure("Main category (Sale/Buy/Rent) is required.");
        }

        if (!IsValidSlMobile(request.MobilePhone))
        {
            return Result<ListingDetailResponse>.Failure("Mobile number must be a valid 10-digit Sri Lankan number.");
        }

        if (!IsValidSlMobile(request.WhatsAppPhone))
        {
            return Result<ListingDetailResponse>.Failure("WhatsApp number must be a valid 10-digit Sri Lankan number.");
        }

        if (string.IsNullOrWhiteSpace(request.Province) || string.IsNullOrWhiteSpace(request.District))
        {
            return Result<ListingDetailResponse>.Failure("Province and district are required.");
        }

        if (request.AdDurationDays < 1 || request.AdDurationDays > 365)
        {
            return Result<ListingDetailResponse>.Failure("Ad duration must be between 1 and 365 days.");
        }

        var minImages = await _appSettingsRepository.GetIntAsync("MinImagesPerListing", 4);
        var maxImages = await _appSettingsRepository.GetIntAsync("MaxImagesPerListing", 10);
        var imageCount = request.Images?.Count ?? 0;
        if (imageCount < minImages)
        {
            return Result<ListingDetailResponse>.Failure($"At least {minImages} photos are required.");
        }
        if (imageCount > maxImages)
        {
            return Result<ListingDetailResponse>.Failure($"Maximum {maxImages} photos allowed.");
        }

        if (await _listingRepository.SlugExistsAsync(request.Slug))
        {
            return Result<ListingDetailResponse>.Failure("Listing slug already exists.");
        }

        if (request.LocationId.HasValue)
        {
            var location = await _locationRepository.GetByIdAsync(request.LocationId.Value);
            if (location is null)
            {
                return Result<ListingDetailResponse>.Failure("Location not found.");
            }
        }

        if (!TryParsePriceType(request.PriceType, out var priceType, out var priceTypeError))
        {
            return Result<ListingDetailResponse>.Failure(priceTypeError!);
        }

        if (!TryParseCondition(request.Condition, out var condition, out var conditionError))
        {
            return Result<ListingDetailResponse>.Failure(conditionError!);
        }

        var perDay = request.ListingPurpose.Trim().ToLowerInvariant() switch
        {
            "buy" => category.PerDayPriceBuy,
            "rent" => category.PerDayPriceRent,
            _ => category.PerDayPriceSale
        };
        var paymentAmount = perDay * request.AdDurationDays;

        var listing = Listing.Create(
            sellerId,
            request.CategoryId,
            request.Title.Trim(),
            request.Slug,
            request.Description.Trim(),
            request.Price,
            request.Currency,
            priceType,
            condition,
            request.LocationId,
            request.City,
            request.District,
            request.Province,
            request.Country,
            NormalizePhone(request.MobilePhone),
            request.ContactEmail,
            request.ShowPhone,
            request.ShowEmail,
            null);

        listing.ApplyPostAdDetails(
            request.ListingPurpose,
            NormalizePhone(request.MobilePhone),
            NormalizePhone(request.WhatsAppPhone),
            request.Address,
            request.AdDurationDays,
            request.Latitude,
            request.Longitude);

        listing.SetPendingPayment(paymentAmount);

        if (request.Images is not null)
        {
            foreach (var image in request.Images)
            {
                listing.AddImage(image.Url, image.ThumbnailUrl, image.AltText, image.SortOrder, image.IsPrimary);
            }
        }

        if (request.Attributes is not null)
        {
            foreach (var attr in request.Attributes)
            {
                var definition = category.Attributes.FirstOrDefault(a => a.Id == attr.CategoryAttributeId);
                if (definition is null)
                {
                    return Result<ListingDetailResponse>.Failure($"Invalid attribute: {attr.CategoryAttributeId}");
                }

                if (definition.IsRequired && string.IsNullOrWhiteSpace(attr.Value))
                {
                    return Result<ListingDetailResponse>.Failure($"{definition.DisplayName} is required.");
                }

                listing.SetAttributeValue(attr.CategoryAttributeId, attr.Value);
            }
        }

        await _listingRepository.AddAsync(listing);

        var created = await _listingRepository.GetByIdAsync(listing.Id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(created!));
    }

    private static bool IsValidSlMobile(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return false;
        }

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.StartsWith("94"))
        {
            digits = digits[2..];
        }
        if (digits.StartsWith('0'))
        {
            digits = digits[1..];
        }

        return digits.Length == 9 && digits[0] is >= '1' and <= '9';
    }

    private static string NormalizePhone(string phone)
    {
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.StartsWith("94"))
        {
            return $"+{digits}";
        }
        if (digits.StartsWith('0'))
        {
            return $"+94{digits[1..]}";
        }
        return $"+94{digits}";
    }

    public async Task<Result<ListingDetailResponse>> GetByIdAsync(Guid id)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        return Result<ListingDetailResponse>.Success(MapToDetailResponse(listing));
    }

    public async Task<Result<ListingDetailResponse>> GetBySlugAsync(string slug)
    {
        var listing = await _listingRepository.GetBySlugAsync(slug, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        return Result<ListingDetailResponse>.Success(MapToDetailResponse(listing));
    }

    public async Task<Result<PagedResult<ListingResponse>>> SearchAsync(ListingSearchRequest request)
    {
        if (request.Page < 1) request = request with { Page = 1 };
        if (request.PageSize < 1 || request.PageSize > 100) request = request with { PageSize = 20 };

        var (items, totalCount) = await _listingRepository.SearchAsync(request);
        var responses = items.Select(MapToResponse).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<PagedResult<ListingResponse>>.Success(
            new PagedResult<ListingResponse>(responses, request.Page, request.PageSize, totalCount, totalPages));
    }

    public async Task<Result<IReadOnlyList<ListingResponse>>> GetFeaturedAsync(int limit = 10)
    {
        var listings = await _listingRepository.GetFeaturedAsync(limit);
        return Result<IReadOnlyList<ListingResponse>>.Success(listings.Select(MapToResponse).ToList());
    }

    public async Task<Result<IReadOnlyList<ListingResponse>>> GetMyListingsAsync(Guid sellerId)
    {
        var listings = await _listingRepository.GetBySellerIdAsync(sellerId);
        return Result<IReadOnlyList<ListingResponse>>.Success(listings.Select(MapToResponse).ToList());
    }

    public async Task<Result<PagedResult<ListingResponse>>> GetSellerListingsAsync(Guid sellerId, int page = 1, int pageSize = 20)
    {
        var request = new ListingSearchRequest(
            SearchTerm: null,
            CategoryId: null,
            LocationId: null,
            City: null,
            Province: null,
            Condition: null,
            MinPrice: null,
            MaxPrice: null,
            Status: nameof(ListingStatus.Active),
            IsFeatured: null,
            SellerId: sellerId,
            Page: page,
            PageSize: pageSize);
        return await SearchAsync(request);
    }

    public async Task<Result<ListingDetailResponse>> UpdateAsync(Guid id, Guid sellerId, bool isAdmin, UpdateListingRequest request)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        if (!isAdmin && listing.SellerId != sellerId)
        {
            return Result<ListingDetailResponse>.Failure("You are not authorized to update this listing.");
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category is null || !category.IsActive)
        {
            return Result<ListingDetailResponse>.Failure("Category not found.");
        }

        if (await _listingRepository.SlugExistsAsync(request.Slug, id))
        {
            return Result<ListingDetailResponse>.Failure("Listing slug already exists.");
        }

        if (!TryParsePriceType(request.PriceType, out var priceType, out var priceTypeError))
        {
            return Result<ListingDetailResponse>.Failure(priceTypeError!);
        }

        if (!TryParseCondition(request.Condition, out var condition, out var conditionError))
        {
            return Result<ListingDetailResponse>.Failure(conditionError!);
        }

        listing.Update(
            request.CategoryId,
            request.Title,
            request.Slug,
            request.Description,
            request.Price,
            request.Currency,
            priceType,
            condition,
            request.LocationId,
            request.City,
            request.District,
            request.Province,
            request.Country,
            request.ContactPhone,
            request.ContactEmail,
            request.ShowPhone,
            request.ShowEmail,
            request.ExpiresAt);

        await _listingRepository.UpdateAsync(listing);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, Guid sellerId, bool isAdmin)
    {
        var listing = await _listingRepository.GetByIdAsync(id);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<bool>.Failure("Listing not found.");
        }

        if (!isAdmin && listing.SellerId != sellerId)
        {
            return Result<bool>.Failure("You are not authorized to delete this listing.");
        }

        listing.SoftDelete();
        await _listingRepository.UpdateAsync(listing);

        return Result<bool>.Success(true);
    }

    public async Task<Result<ListingDetailResponse>> SubmitForReviewAsync(Guid id, Guid sellerId, bool isAdmin)
    {
        var listing = await GetAuthorizedListingAsync(id, sellerId, isAdmin);
        if (!listing.IsSuccess)
        {
            return Result<ListingDetailResponse>.Failure(listing.Error!);
        }

        listing.Value!.SubmitForReview();
        await _listingRepository.UpdateAsync(listing.Value);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<ListingDetailResponse>> PublishAsync(Guid id)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        listing.Publish();
        await _listingRepository.UpdateAsync(listing);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<ListingDetailResponse>> RejectAsync(Guid id)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        listing.Reject();
        await _listingRepository.UpdateAsync(listing);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<ListingDetailResponse>> MarkAsSoldAsync(Guid id, Guid sellerId, bool isAdmin)
    {
        var listing = await GetAuthorizedListingAsync(id, sellerId, isAdmin);
        if (!listing.IsSuccess)
        {
            return Result<ListingDetailResponse>.Failure(listing.Error!);
        }

        listing.Value!.MarkAsSold();
        await _listingRepository.UpdateAsync(listing.Value);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<ListingDetailResponse>> FeatureAsync(Guid id, FeatureListingRequest request)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        listing.Feature(request.FeaturedUntil);
        await _listingRepository.UpdateAsync(listing);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<ListingDetailResponse>> RemoveFeaturedAsync(Guid id)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        listing.RemoveFeatured();
        await _listingRepository.UpdateAsync(listing);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    public async Task<Result<ListingImageResponse>> AddImageAsync(Guid listingId, Guid sellerId, bool isAdmin, AddListingImageRequest request)
    {
        var listing = await GetAuthorizedListingAsync(listingId, sellerId, isAdmin);
        if (!listing.IsSuccess)
        {
            return Result<ListingImageResponse>.Failure(listing.Error!);
        }

        var image = listing.Value!.AddImage(request.Url, request.ThumbnailUrl, request.AltText, request.SortOrder, request.IsPrimary);
        await _listingRepository.UpdateAsync(listing.Value);

        return Result<ListingImageResponse>.Success(MapImage(image));
    }

    public async Task<Result<bool>> RemoveImageAsync(Guid listingId, Guid imageId, Guid sellerId, bool isAdmin)
    {
        var listing = await GetAuthorizedListingAsync(listingId, sellerId, isAdmin);
        if (!listing.IsSuccess)
        {
            return Result<bool>.Failure(listing.Error!);
        }

        try
        {
            listing.Value!.RemoveImage(imageId);
        }
        catch (InvalidOperationException)
        {
            return Result<bool>.Failure("Image not found.");
        }

        await _listingRepository.UpdateAsync(listing.Value);
        return Result<bool>.Success(true);
    }

    public async Task<Result<ListingDetailResponse>> IncrementViewCountAsync(Guid id)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status != ListingStatus.Active)
        {
            return Result<ListingDetailResponse>.Failure("Listing not found.");
        }

        listing.IncrementViewCount();
        await _listingRepository.UpdateAsync(listing);

        var updated = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        return Result<ListingDetailResponse>.Success(MapToDetailResponse(updated!));
    }

    private async Task<Result<Listing>> GetAuthorizedListingAsync(Guid id, Guid sellerId, bool isAdmin)
    {
        var listing = await _listingRepository.GetByIdAsync(id, includeDetails: true);
        if (listing is null || listing.Status == ListingStatus.Deleted)
        {
            return Result<Listing>.Failure("Listing not found.");
        }

        if (!isAdmin && listing.SellerId != sellerId)
        {
            return Result<Listing>.Failure("You are not authorized to manage this listing.");
        }

        return Result<Listing>.Success(listing);
    }

    private static bool TryParsePriceType(string value, out PriceType priceType, out string? error)
    {
        error = null;
        if (Enum.TryParse<PriceType>(value, ignoreCase: true, out priceType))
        {
            return true;
        }

        error = "Invalid price type.";
        return false;
    }

    private static bool TryParseCondition(string value, out ListingCondition condition, out string? error)
    {
        error = null;
        if (Enum.TryParse<ListingCondition>(value, ignoreCase: true, out condition))
        {
            return true;
        }

        error = "Invalid condition.";
        return false;
    }

    private static ListingResponse MapToResponse(Listing listing) => new(
        listing.Id,
        listing.SellerId,
        listing.CategoryId,
        listing.Category?.Name ?? string.Empty,
        listing.Title,
        listing.Slug,
        listing.Description,
        listing.Price,
        listing.Currency,
        listing.PriceType.ToString(),
        listing.Condition.ToString(),
        listing.Status.ToString(),
        listing.LocationId,
        listing.City,
        listing.District,
        listing.Province,
        listing.Country,
        listing.ShowPhone ? listing.ContactPhone : null,
        listing.ShowEmail ? listing.ContactEmail : null,
        listing.ShowPhone,
        listing.ShowEmail,
        listing.ViewCount,
        listing.IsFeatured,
        listing.FeaturedUntil,
        listing.PublishedAt,
        listing.ExpiresAt,
        listing.Images.FirstOrDefault(i => i.IsPrimary)?.Url ?? listing.Images.FirstOrDefault()?.Url,
        listing.CreatedAt,
        listing.UpdatedAt);

    private static ListingDetailResponse MapToDetailResponse(Listing listing) => new(
        listing.Id,
        listing.SellerId,
        listing.CategoryId,
        listing.Category?.Name ?? string.Empty,
        listing.Title,
        listing.Slug,
        listing.Description,
        listing.Price,
        listing.Currency,
        listing.PriceType.ToString(),
        listing.Condition.ToString(),
        listing.Status.ToString(),
        listing.LocationId,
        listing.Location?.Name,
        listing.City,
        listing.District,
        listing.Province,
        listing.Country,
        listing.Latitude,
        listing.Longitude,
        listing.ShowPhone ? listing.ContactPhone : null,
        listing.ShowEmail ? listing.ContactEmail : null,
        listing.ShowPhone,
        listing.ShowEmail,
        listing.ViewCount,
        listing.IsFeatured,
        listing.FeaturedUntil,
        listing.PublishedAt,
        listing.ExpiresAt,
        listing.Images.OrderBy(i => i.SortOrder).Select(MapImage).ToList(),
        listing.AttributeValues.Select(a => new ListingAttributeValueResponse(
            a.Id,
            a.CategoryAttributeId,
            a.CategoryAttribute?.Name ?? string.Empty,
            a.CategoryAttribute?.DisplayName ?? string.Empty,
            a.Value)).ToList(),
        listing.CreatedAt,
        listing.UpdatedAt);

    private static ListingImageResponse MapImage(ListingImage image) => new(
        image.Id,
        image.Url,
        image.ThumbnailUrl,
        image.AltText,
        image.SortOrder,
        image.IsPrimary);
}
