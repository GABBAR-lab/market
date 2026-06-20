using ProfileService.Domain.Entities;

namespace ProfileService.Application.Interfaces;

public interface IProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id);
    Task<UserProfile?> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<UserProfile>> GetAllAsync();
    Task AddAsync(UserProfile profile);
    Task UpdateAsync(UserProfile profile);
    Task<bool> UserIdExistsAsync(Guid userId);
}
