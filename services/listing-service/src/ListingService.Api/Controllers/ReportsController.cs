using System.Security.Claims;
using ListingService.Application.DTOs.Reports;
using ListingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListingService.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private static readonly HashSet<string> ValidReasons =
    [
        "Spam", "Fraud", "WrongCategory", "Duplicate", "Offensive", "Other"
    ];

    private readonly IReportRepository _reportRepository;
    private readonly IListingRepository _listingRepository;

    public ReportsController(IReportRepository reportRepository, IListingRepository listingRepository)
    {
        _reportRepository = reportRepository;
        _listingRepository = listingRepository;
    }

    [HttpPost("listings/{listingId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> ReportListing(Guid listingId, [FromBody] CreateListingReportRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason) || !ValidReasons.Contains(request.Reason))
        {
            return BadRequest(new { error = "Invalid report reason." });
        }

        var listing = await _listingRepository.GetByIdAsync(listingId);
        if (listing is null)
        {
            return NotFound(new { error = "Listing not found." });
        }

        Guid? reporterId = null;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (Guid.TryParse(claim, out var userId))
        {
            reporterId = userId;
        }

        var id = await _reportRepository.CreateAsync(listingId, reporterId, request.Reason.Trim(), request.Comment?.Trim());
        return Ok(new ListingReportResponse(id, listingId, request.Reason, "Pending", DateTime.UtcNow));
    }
}
