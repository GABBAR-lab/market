using ProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProfileService.Infrastructure.Persistence.Configurations;

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Label)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.StreetLine1)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.StreetLine2)
            .HasMaxLength(200);

        builder.Property(a => a.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();
    }
}
