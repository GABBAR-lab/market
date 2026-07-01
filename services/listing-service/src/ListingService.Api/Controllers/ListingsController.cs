using System.Security.Claims;
using ListingService.Application.DTOs.Listings;
using ListingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly IListingService _listingService;

    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ListingSearchRequest request)
    {
        var result = await _listingService.SearchAsync(request);
        return Ok(result.Value);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int limit = 10)
    {
        var result = await _listingService.GetFeaturedAsync(limit);
        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyListings()
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.GetMyListingsAsync(userId);
        return Ok(result.Value);
    }

    [HttpGet("seller/{sellerId:guid}")]
    public async Task<IActionResult> GetBySeller(Guid sellerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _listingService.GetSellerListingsAsync(sellerId, page, pageSize);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _listingService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _listingService.GetBySlugAsync(slug);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateListingRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.CreateAsync(userId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListingRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.UpdateAsync(id, userId, User.IsInRole("Admin"), request);
        if (!result.IsSuccess)
        {
            return result.Error!.Contains("authorized") ? Forbid() : NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.DeleteAsync(id, userId, User.IsInRole("Admin"));
        if (!result.IsSuccess)
        {
            return result.Error!.Contains("authorized") ? Forbid() : NotFound(new { error = result.Error });
        }

        return Ok(new { message = "Listing deleted successfully." });
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize]
    public async Task<IActionResult> SubmitForReview(Guid id)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.SubmitForReviewAsync(id, userId, User.IsInRole("Admin"));
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await _listingService.PublishAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await _listingService.RejectAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/activate-after-payment")]
    [AllowAnonymous]
    public async Task<IActionResult> ActivateAfterPayment(Guid id, [FromBody] ActivateAfterPaymentRequest request)
    {
        var result = await _listingService.ActivateAfterPaymentAsync(id, request.RequireApproval);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/sold")]
    [Authorize]
    public async Task<IActionResult> MarkAsSold(Guid id)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.MarkAsSoldAsync(id, userId, User.IsInRole("Admin"));
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/feature")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Feature(Guid id, [FromBody] FeatureListingRequest request)
    {
        var result = await _listingService.FeatureAsync(id, request);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/unfeature")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveFeatured(Guid id)
    {
        var result = await _listingService.RemoveFeaturedAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{listingId:guid}/images")]
    [Authorize]
    public async Task<IActionResult> AddImage(Guid listingId, [FromBody] AddListingImageRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.AddImageAsync(listingId, userId, User.IsInRole("Admin"), request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{listingId:guid}/images/{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveImage(Guid listingId, Guid imageId)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _listingService.RemoveImageAsync(listingId, imageId, userId, User.IsInRole("Admin"));
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Image removed successfully." });
    }

    [HttpPost("{id:guid}/view")]
    public async Task<IActionResult> IncrementViewCount(Guid id)
    {
        var result = await _listingService.IncrementViewCountAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out userId);
    }
}
