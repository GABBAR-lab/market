using System.Security.Claims;
using ListingService.Application.DTOs.Inquiries;
using ListingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

[ApiController]
[Route("api/inquiries")]
public class InquiriesController : ControllerBase
{
    private readonly IInquiryRepository _inquiryRepository;
    private readonly IListingRepository _listingRepository;

    public InquiriesController(IInquiryRepository inquiryRepository, IListingRepository listingRepository)
    {
        _inquiryRepository = inquiryRepository;
        _listingRepository = listingRepository;
    }

    [HttpPost("listings/{listingId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Create(Guid listingId, [FromBody] CreateInquiryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BuyerName) || string.IsNullOrWhiteSpace(request.BuyerPhone) || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Name, phone and message are required." });
        }

        var listing = await _listingRepository.GetByIdAsync(listingId);
        if (listing is null)
        {
            return NotFound(new { error = "Listing not found." });
        }

        Guid? buyerId = null;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (Guid.TryParse(claim, out var userId))
        {
            buyerId = userId;
        }

        var id = await _inquiryRepository.CreateAsync(
            listingId,
            buyerId,
            request.BuyerName.Trim(),
            request.BuyerPhone.Trim(),
            request.Message.Trim());

        return Ok(new { id, message = "Your message was sent to the seller." });
    }

    [HttpGet("me")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetMyInquiries()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var items = await _inquiryRepository.GetBySellerAsync(userId);
        return Ok(items);
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}
