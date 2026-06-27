using System.Security.Claims;
using ProfileService.Application.DTOs.Profiles;
using ProfileService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProfileService.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _profileService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _profileService.GetByUserIdAsync(userId);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("public/seller/{userId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicSeller(Guid userId)
    {
        var result = await _profileService.GetPublicSellerProfileAsync(userId);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _profileService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _profileService.GetByUserIdAsync(userId);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateProfileRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        if (request.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var result = await _profileService.CreateAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileRequest request)
    {
        if (!await CanManageProfileAsync(id))
        {
            return Forbid();
        }

        var result = await _profileService.UpdateAsync(id, request);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await CanManageProfileAsync(id))
        {
            return Forbid();
        }

        var result = await _profileService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(new { message = "Profile deleted successfully." });
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _profileService.ActivateAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _profileService.DeactivateAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{profileId:guid}/addresses")]
    [Authorize]
    public async Task<IActionResult> AddAddress(Guid profileId, [FromBody] CreateAddressRequest request)
    {
        if (!await CanManageProfileAsync(profileId))
        {
            return Forbid();
        }

        var result = await _profileService.AddAddressAsync(profileId, request);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPut("{profileId:guid}/addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateAddress(
        Guid profileId,
        Guid addressId,
        [FromBody] UpdateAddressRequest request)
    {
        if (!await CanManageProfileAsync(profileId))
        {
            return Forbid();
        }

        var result = await _profileService.UpdateAddressAsync(profileId, addressId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{profileId:guid}/addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveAddress(Guid profileId, Guid addressId)
    {
        if (!await CanManageProfileAsync(profileId))
        {
            return Forbid();
        }

        var result = await _profileService.RemoveAddressAsync(profileId, addressId);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Address removed successfully." });
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(claim, out userId);
    }

    private async Task<bool> CanManageProfileAsync(Guid profileId)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        if (!TryGetCurrentUserId(out var userId))
        {
            return false;
        }

        var profile = await _profileService.GetByIdAsync(profileId);
        return profile.IsSuccess && profile.Value!.UserId == userId;
    }
}
