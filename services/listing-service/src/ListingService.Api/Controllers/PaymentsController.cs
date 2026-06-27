using System.Security.Claims;
using ListingService.Application.DTOs.Payments;
using ListingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("calculate")]
    [AllowAnonymous]
    public async Task<IActionResult> Calculate([FromBody] CalculatePaymentRequest request)
    {
        var result = await _paymentService.CalculateAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("complete")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Complete([FromBody] CompletePaymentRequest request)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(new { error = "Invalid user token." });
        }

        var result = await _paymentService.CompletePaymentAsync(userId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public AdminCategoriesController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("pricing")]
    public async Task<IActionResult> GetPricing()
    {
        var result = await _paymentService.GetAllCategoryPricingAsync();
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/pricing")]
    public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] UpdateCategoryPricingRequest request)
    {
        var result = await _paymentService.UpdateCategoryPricingAsync(id, request);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IAppSettingsRepository _settings;

    public MediaController(IWebHostEnvironment env, IAppSettingsRepository settings)
    {
        _env = env;
        _settings = settings;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Seller,Admin")]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
        {
            return BadRequest(new { error = "No files uploaded." });
        }

        var maxImages = await _settings.GetIntAsync("MaxImagesPerListing", 10);
        if (files.Count > maxImages)
        {
            return BadRequest(new { error = $"Maximum {maxImages} images allowed per upload." });
        }

        var uploadDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "listings");
        Directory.CreateDirectory(uploadDir);

        var urls = new List<string>();
        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                continue;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext is not ".jpg" and not ".jpeg" and not ".png" and not ".webp")
            {
                return BadRequest(new { error = "Only JPG, JPEG, PNG, and WEBP files are allowed." });
            }

            var fileName = $"{Guid.NewGuid():N}.webp";
            var path = Path.Combine(uploadDir, fileName);

            await using var stream = file.OpenReadStream();
            await using var outStream = System.IO.File.Create(path);
            await stream.CopyToAsync(outStream);

            urls.Add($"/uploads/listings/{fileName}");
        }

        return Ok(new { urls });
    }
}
