using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Infrastructure.Http;

public class ListingServiceClient : IListingServiceClient
{
    private readonly HttpClient _http;

    public ListingServiceClient(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        _http.BaseAddress = new Uri(configuration["Services:Listing"] ?? "http://localhost:5003");
    }

    public async Task<ListingCategoryDto?> GetCategoryAsync(Guid categoryId)
    {
        var cat = await _http.GetFromJsonAsync<CategoryApiResponse>($"/api/categories/{categoryId}");
        return cat is null ? null : new ListingCategoryDto(cat.Id, cat.Name, cat.PerDayPriceSale, cat.PerDayPriceBuy, cat.PerDayPriceRent, cat.IsActive);
    }

    public async Task<ListingForPaymentDto?> GetListingAsync(Guid listingId)
    {
        var l = await _http.GetFromJsonAsync<ListingApiResponse>($"/api/listings/{listingId}");
        return l is null ? null : new ListingForPaymentDto(l.Id, l.SellerId, l.CategoryId, l.Status, l.ListingPurpose, l.AdDurationDays, l.PaymentAmount);
    }

    public async Task<bool> ActivateListingAfterPaymentAsync(Guid listingId, bool requireApproval)
    {
        var response = await _http.PostAsJsonAsync($"/api/listings/{listingId}/activate-after-payment", new { requireApproval });
        return response.IsSuccessStatusCode;
    }

    public async Task<IReadOnlyList<CategoryPricingResponse>> GetCategoryPricingAsync()
    {
        var items = await _http.GetFromJsonAsync<List<CategoryPricingResponse>>("/api/admin/categories/pricing");
        return items ?? [];
    }

    public async Task<CategoryPricingResponse?> UpdateCategoryPricingAsync(Guid categoryId, UpdateCategoryPricingRequest request)
    {
        var response = await _http.PutAsJsonAsync($"/api/admin/categories/{categoryId}/pricing", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<CategoryPricingResponse>() : null;
    }

    private record CategoryApiResponse(Guid Id, string Name, decimal PerDayPriceSale, decimal PerDayPriceBuy, decimal PerDayPriceRent, bool IsActive);
    private record ListingApiResponse(Guid Id, Guid SellerId, Guid CategoryId, string Status, string? ListingPurpose, int AdDurationDays, decimal? PaymentAmount);
}
