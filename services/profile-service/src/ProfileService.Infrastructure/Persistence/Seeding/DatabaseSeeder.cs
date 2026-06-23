using ProfileService.Domain.Entities;
using ProfileService.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;

namespace ProfileService.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ProfileDbContext context)
    {
        if (await context.UserProfiles.AnyAsync())
        {
            return;
        }

        var adminProfile = UserProfile.Create(
            SeedUserIds.Admin,
            "System",
            "Admin",
            bio: "Platform administrator",
            phoneNumber: "0112345678",
            language: "en",
            currency: "LKR",
            timezone: "Asia/Colombo");

        var sellerProfile = UserProfile.Create(
            SeedUserIds.Seller,
            "Demo",
            "Seller",
            bio: "Trusted seller on Living Lanka",
            phoneNumber: "0771234567",
            language: "en",
            currency: "LKR",
            timezone: "Asia/Colombo");

        await context.UserProfiles.AddRangeAsync(adminProfile, sellerProfile);
        await context.SaveChangesAsync();
    }
}
