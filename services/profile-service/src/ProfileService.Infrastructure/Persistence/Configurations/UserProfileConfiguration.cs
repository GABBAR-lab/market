using ProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProfileService.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.Property(p => p.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Bio)
            .HasMaxLength(1000);

        builder.Property(p => p.AvatarUrl)
            .HasMaxLength(512);

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(p => p.Website)
            .HasMaxLength(256);

        builder.Property(p => p.Language)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Timezone)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasConversion<int?>();

        builder.Property(p => p.Status)
            .HasConversion<int>();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Navigation(p => p.Addresses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Addresses)
            .WithOne(a => a.UserProfile)
            .HasForeignKey(a => a.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
