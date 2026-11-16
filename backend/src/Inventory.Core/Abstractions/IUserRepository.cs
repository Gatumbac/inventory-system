using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<PagedResult<User>> GetPagedAsync(UserFilterRequest filterRequest);
    Task<User?> GetByIdForUpdateAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<Role> GetRoleByNameAsync(string name);
    Task<int> CountAdminsAsync();
    Task AddAsync(User user);
    Task RemoveAsync(User user);
}
