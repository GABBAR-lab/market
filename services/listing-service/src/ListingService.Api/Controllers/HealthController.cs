using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

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
            service = "ListingService",
            timestamp = DateTime.UtcNow
        });
    }
}
