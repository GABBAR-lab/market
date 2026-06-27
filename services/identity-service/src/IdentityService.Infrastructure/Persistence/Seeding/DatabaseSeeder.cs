using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IdentityDbContext context, IPasswordHasher passwordHasher)
    {
        if (!await context.Roles.AnyAsync())
        {
            await SeedRolesAndUsersAsync(context, passwordHasher);
        }

        await EnsureSellerRoleForAllUsersAsync(context);
    }

    private static async Task SeedRolesAndUsersAsync(IdentityDbContext context, IPasswordHasher passwordHasher)
    {
        var adminRole = Role.CreateSystemRole("Admin", "Platform administrator");
        var sellerRole = Role.CreateSystemRole("Seller", "Marketplace seller");
        var buyerRole = Role.CreateSystemRole("Buyer", "Marketplace buyer");

        await context.Roles.AddRangeAsync(adminRole, sellerRole, buyerRole);

        var adminUser = User.CreateWithId(
            SeedUserIds.Admin,
            "admin@marketplace.com",
            passwordHasher.Hash("Admin@123"),
            "System",
            "Admin");
        adminUser.VerifyEmail();
        adminUser.AssignRole(adminRole);
        adminUser.AssignRole(sellerRole);

        var sellerUser = User.CreateWithId(
            SeedUserIds.Seller,
            "seller@marketplace.com",
            passwordHasher.Hash("Seller@123"),
            "Demo",
            "Seller",
            "0771234567");
        sellerUser.VerifyEmail();
        sellerUser.AssignRole(sellerRole);

        await context.Users.AddRangeAsync(adminUser, sellerUser);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureSellerRoleForAllUsersAsync(IdentityDbContext context)
    {
        var sellerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Seller");
        if (sellerRole is null)
        {
            return;
        }

        var userIdsWithoutSeller = await context.Users
            .AsNoTracking()
            .Where(u => u.Status != UserStatus.Deleted)
            .Where(u => !context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == sellerRole.Id))
            .Select(u => u.Id)
            .ToListAsync();

        if (userIdsWithoutSeller.Count == 0)
        {
            return;
        }

        foreach (var userId in userIdsWithoutSeller)
        {
            await context.UserRoles.AddAsync(UserRole.Assign(userId, sellerRole.Id));
        }

        await context.SaveChangesAsync();
    }
}
