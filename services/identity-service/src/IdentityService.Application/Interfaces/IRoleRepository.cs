using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<IReadOnlyList<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
}
