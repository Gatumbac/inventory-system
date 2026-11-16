using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
}
