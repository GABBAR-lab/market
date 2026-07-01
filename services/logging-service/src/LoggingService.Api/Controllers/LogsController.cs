using LoggingService.Application.DTOs;
using LoggingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoggingService.Api.Controllers;

[ApiController]
[Route("api/logs")]
[Authorize(Roles = "Admin")]
public class LogsController : ControllerBase
{
    private readonly ILogAppService _logs;

    public LogsController(ILogAppService logs) => _logs = logs;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] LogSearchRequest request)
    {
        var (items, total) = await _logs.SearchAsync(request);
        return Ok(new { items, total, page = request.Page, pageSize = request.PageSize });
    }
}

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", service = "LoggingService", timestamp = DateTime.UtcNow });
}
