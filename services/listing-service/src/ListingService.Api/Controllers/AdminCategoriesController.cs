using ListingService.Application.DTOs.Payments;
using ListingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize(Roles = "Admin")]
public class AdminCategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categories;

    public AdminCategoriesController(ICategoryRepository categories) => _categories = categories;

    [HttpGet("pricing")]
    public async Task<IActionResult> GetPricing()
    {
        var items = await _categories.GetRootCategoryPricingAsync();
        return Ok(items);
    }

    [HttpPut("{id:guid}/pricing")]
    public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] UpdateCategoryPricingRequest request)
    {
        if (request.PerDayPriceSale < 0 || request.PerDayPriceBuy < 0 || request.PerDayPriceRent < 0)
        {
            return BadRequest(new { error = "Prices cannot be negative." });
        }

        var updated = await _categories.UpdatePricingAsync(
            id, request.PerDayPriceSale, request.PerDayPriceBuy, request.PerDayPriceRent);

        return updated is null ? NotFound(new { error = "Category not found." }) : Ok(updated);
    }
}
