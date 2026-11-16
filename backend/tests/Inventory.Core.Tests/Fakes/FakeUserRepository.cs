using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Tests.Fakes;

public class FakeUserRepository : IUserRepository
{
    private int _nextId = 1;

    public List<User> Users { get; } = new List<User>();
    public List<User> Removed { get; } = new List<User>();

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

    public Task<PagedResult<User>> GetPagedAsync(UserFilterRequest filterRequest)
    {
        var page = new PagedResult<User>()
        {
            Items = Users.ToList(),
            Page = filterRequest.Page,
            PageSize = filterRequest.PageSize,
            TotalCount = Users.Count
        };

        return Task.FromResult(page);
    }

    public Task<User?> GetByIdForUpdateAsync(int id)
    {
        return GetByIdAsync(id);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return Task.FromResult(Users.Any(u => u.Email == email));
    }

    public Task<Role> GetRoleByNameAsync(string name)
    {
        var role = new Role()
        {
            Name = name
        };

        return Task.FromResult(role);
    }

    public Task<int> CountAdminsAsync()
    {
        return Task.FromResult(Users.Count(u => u.Role.Name == "Admin"));
    }

    public Task AddAsync(User user)
    {
        user.Id = _nextId++;
        Users.Add(user);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(User user)
    {
        Users.Remove(user);
        Removed.Add(user);
        return Task.CompletedTask;
    }
}
