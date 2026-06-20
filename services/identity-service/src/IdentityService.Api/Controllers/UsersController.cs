using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateAsync(id, request);
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
        var result = await _userService.DeleteAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(new { message = "User deleted successfully." });
    }

    [HttpPost("{id:guid}/verify-email")]
    public async Task<IActionResult> VerifyEmail(Guid id)
    {
        var result = await _userService.VerifyEmailAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/suspend")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Suspend(Guid id)
    {
        var result = await _userService.SuspendAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _userService.ActivateAsync(id);
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId)
    {
        var result = await _userService.AssignRoleAsync(userId, roleId);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
    {
        var result = await _userService.RemoveRoleAsync(userId, roleId);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}
