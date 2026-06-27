namespace ListingService.Application.DTOs.Reports;

public record CreateListingReportRequest(
    string Reason,
    string? Comment);

public record ListingReportResponse(
    Guid Id,
    Guid ListingId,
    string Reason,
    string Status,
    DateTime CreatedAt);
