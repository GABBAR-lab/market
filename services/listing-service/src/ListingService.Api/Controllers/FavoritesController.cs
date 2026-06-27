using System.Security.Claims;
using ListingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IListingRepository _listingRepository;

    public FavoritesController(IFavoriteRepository favoriteRepository, IListingRepository listingRepository)
    {
        _favoriteRepository = favoriteRepository;
        _listingRepository = listingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var ids = await _favoriteRepository.GetListingIdsByUserAsync(userId);
        return Ok(ids);
    }

    [HttpPost("{listingId:guid}")]
    public async Task<IActionResult> Add(Guid listingId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var listing = await _listingRepository.GetByIdAsync(listingId);
        if (listing is null)
        {
            return NotFound(new { error = "Listing not found." });
        }

        await _favoriteRepository.AddAsync(userId, listingId);
        return Ok(new { message = "Saved to favorites." });
    }

    [HttpDelete("{listingId:guid}")]
    public async Task<IActionResult> Remove(Guid listingId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        await _favoriteRepository.RemoveAsync(userId, listingId);
        return Ok(new { message = "Removed from favorites." });
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}
