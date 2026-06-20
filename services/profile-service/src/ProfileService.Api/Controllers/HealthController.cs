using Microsoft.AspNetCore.Mvc;

namespace ProfileService.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            service = "ProfileService",
            timestamp = DateTime.UtcNow
        });
    }
}
