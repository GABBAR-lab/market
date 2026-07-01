using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Interfaces;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationAppService _notifications;

    public NotificationsController(INotificationAppService notifications) => _notifications = notifications;

    [HttpGet("me")]
    public async Task<IActionResult> GetMine([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var items = await _notifications.GetMyNotificationsAsync(userId, page, pageSize);
        return Ok(items);
    }

    [HttpGet("me/unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var count = await _notifications.GetUnreadCountAsync(userId);
        return Ok(new { count });
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        await _notifications.MarkAsReadAsync(userId, id);
        return Ok(new { message = "Marked as read." });
    }

    [HttpPost("me/read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        await _notifications.MarkAllAsReadAsync(userId);
        return Ok(new { message = "All notifications marked as read." });
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", service = "NotificationService", timestamp = DateTime.UtcNow });
}
