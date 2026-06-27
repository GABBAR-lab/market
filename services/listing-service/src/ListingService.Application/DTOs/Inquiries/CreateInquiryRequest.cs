namespace ListingService.Application.DTOs.Inquiries;

public record CreateInquiryRequest(
    string BuyerName,
    string BuyerPhone,
    string Message);
