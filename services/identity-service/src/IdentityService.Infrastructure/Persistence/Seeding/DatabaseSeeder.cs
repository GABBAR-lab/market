using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IdentityDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var adminRole = Role.CreateSystemRole("Admin", "Platform administrator");
        var sellerRole = Role.CreateSystemRole("Seller", "Marketplace seller");
        var buyerRole = Role.CreateSystemRole("Buyer", "Marketplace buyer");

        await context.Roles.AddRangeAsync(adminRole, sellerRole, buyerRole);

        var adminUser = User.Create(
            "admin@marketplace.com",
            passwordHasher.Hash("Admin@123"),
            "System",
            "Admin");

        adminUser.VerifyEmail();
        adminUser.AssignRole(adminRole);

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}
