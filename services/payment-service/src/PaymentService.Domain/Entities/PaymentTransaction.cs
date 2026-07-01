namespace PaymentService.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Guid SellerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "LKR";
    public int DurationDays { get; set; }
    public decimal PerDayPrice { get; set; }
    public string Status { get; set; } = "Completed";
    public string PaymentMethod { get; set; } = "Card";
    public string? TransactionRef { get; set; }
    public string? CardLastFour { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
