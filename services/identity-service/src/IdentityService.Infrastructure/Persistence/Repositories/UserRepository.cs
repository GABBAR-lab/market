using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    private IQueryable<User> UsersWithDetails =>
        _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.RefreshTokens);

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await UsersWithDetails.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await UsersWithDetails.FirstOrDefaultAsync(u => u.Email == normalized);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await UsersWithDetails
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await UsersWithDetails.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        foreach (var refreshToken in user.RefreshTokens)
        {
            var existsInDatabase = await _context.RefreshTokens
                .AsNoTracking()
                .AnyAsync(t => t.Id == refreshToken.Id);

            if (!existsInDatabase)
            {
                await _context.RefreshTokens.AddAsync(refreshToken);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _context.Users.AnyAsync(u => u.Email == normalized);
    }
}
