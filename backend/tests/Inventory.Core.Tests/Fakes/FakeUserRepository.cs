using Inventory.Core.Abstractions;
using Inventory.Core.Entities;

namespace Inventory.Core.Tests.Fakes;

public class FakeUserRepository : IUserRepository
{
    private int _nextId = 1;

    public List<User> Users { get; } = new List<User>();

    public User Seed(string name, string lastName, string email, string roleName)
    {
        var user = new User()
        {
            Id = _nextId++,
            Name = name,
            LastName = lastName,
            Email = email,
            PasswordHash = "hash",
            Role = new Role()
            {
                Name = roleName
            }
        };

        Users.Add(user);
        return user;
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return Task.FromResult(Users.FirstOrDefault(u => u.Email == email));
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
    }
}
