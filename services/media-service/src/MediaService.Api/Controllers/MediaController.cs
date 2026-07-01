using System.Security.Claims;
using MediaService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaAppService _mediaService;
    private readonly IMediaRepository _repository;

    public MediaController(IMediaAppService mediaService, IMediaRepository repository)
    {
        _mediaService = mediaService;
        _repository = repository;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Seller,Admin")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> UploadListings([FromForm] List<IFormFile> files)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var max = await _repository.GetSettingIntAsync("MaxImagesPerListing", 10);
        return await UploadInternal(userId, "listings", files, max);
    }

    [HttpPost("avatars")]
    [Authorize]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var files = file is null ? [] : new List<IFormFile> { file };
        return await UploadInternal(userId, "avatars", files, 1);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyMedia([FromQuery] string? category)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _mediaService.GetMyMediaAsync(userId, category);
        return Ok(result.Value);
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllMedia([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _mediaService.GetAllMediaAsync(page, pageSize);
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");
        var result = await _mediaService.DeleteAsync(id, userId, isAdmin);
        if (!result.IsSuccess)
        {
            return result.Error!.Contains("authorized") ? Forbid() : NotFound(new { error = result.Error });
        }

        return Ok(new { message = "Media deleted." });
    }

    private async Task<IActionResult> UploadInternal(Guid userId, string category, List<IFormFile> files, int maxCount)
    {
        var payloads = files
            .Where(f => f.Length > 0)
            .Select(f => (f.OpenReadStream(), f.FileName, f.Length))
            .ToList();

        var result = await _mediaService.UploadAsync(userId, category, payloads, maxCount);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { urls = result.Value!.Urls, assets = result.Value.Assets });
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}
