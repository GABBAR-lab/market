using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<PaymentTransaction> Payments => Set<PaymentTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentTransaction>(e =>
        {
            e.ToTable("Payments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.PerDayPrice).HasPrecision(18, 2);
            e.Property(x => x.Currency).HasMaxLength(10);
            e.Property(x => x.Status).HasMaxLength(50);
            e.HasIndex(x => x.ListingId);
            e.HasIndex(x => x.SellerId);
        });
    }
}

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _db;

    public PaymentRepository(PaymentDbContext db) => _db = db;

    public async Task<PaymentTransaction> AddAsync(PaymentTransaction payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
        return payment;
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetBySellerAsync(Guid sellerId) =>
        await _db.Payments.Where(p => p.SellerId == sellerId).OrderByDescending(p => p.CreatedAt).ToListAsync();
}
