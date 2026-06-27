namespace ListingService.Application.DTOs.Listings;

public record SetListingAttributeRequest(
    Guid CategoryAttributeId,
    string Value);
