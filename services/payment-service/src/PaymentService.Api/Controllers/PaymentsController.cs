using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentAppService _payments;

    public PaymentsController(IPaymentAppService payments) => _payments = payments;

    [HttpPost("calculate")]
    [AllowAnonymous]
    public async Task<IActionResult> Calculate([FromBody] CalculatePaymentRequest request)
    {
        var result = await _payments.CalculateAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("complete")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Complete([FromBody] CompletePaymentRequest request)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _payments.CompletePaymentAsync(userId, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly IPaymentAppService _payments;

    public AdminCategoriesController(IPaymentAppService payments) => _payments = payments;

    [HttpGet("pricing")]
    public async Task<IActionResult> GetPricing()
    {
        var result = await _payments.GetAllCategoryPricingAsync();
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/pricing")]
    public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] UpdateCategoryPricingRequest request)
    {
        var result = await _payments.UpdateCategoryPricingAsync(id, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", service = "PaymentService", timestamp = DateTime.UtcNow });
}
