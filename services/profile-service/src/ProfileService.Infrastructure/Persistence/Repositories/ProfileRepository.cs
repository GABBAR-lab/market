using ProfileService.Application.Interfaces;
using ProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProfileService.Infrastructure.Persistence.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileDbContext _context;

    public ProfileRepository(ProfileDbContext context)
    {
        _context = context;
    }

    private IQueryable<UserProfile> ProfilesWithAddresses =>
        _context.UserProfiles
            .Include(p => p.Addresses);

    public async Task<UserProfile?> GetByIdAsync(Guid id)
    {
        return await ProfilesWithAddresses.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return await ProfilesWithAddresses.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<IReadOnlyList<UserProfile>> GetAllAsync()
    {
        return await ProfilesWithAddresses.ToListAsync();
    }

    public async Task AddAsync(UserProfile profile)
    {
        await _context.UserProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile profile)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserIdExistsAsync(Guid userId)
    {
        return await _context.UserProfiles.AnyAsync(p => p.UserId == userId);
    }
}
