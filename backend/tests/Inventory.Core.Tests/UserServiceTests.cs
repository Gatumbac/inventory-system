using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Inventory.Core.Services;
using Inventory.Core.Tests.Fakes;

namespace Inventory.Core.Tests;

public class UserServiceTests
{
    private readonly FakeUserRepository _users = new FakeUserRepository();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_users, new StubPasswordHasher());
    }

    [Fact]
    public async Task CreateUser_NormalizesEmailAndHashesPassword()
    {
        var result = await _service.CreateUserAsync(NewRequest("  Ana@Example.com "));

        Assert.Equal(SaveUserStatus.Saved, result.Status);
        Assert.Equal("ana@example.com", result.User!.Email);
        Assert.Equal("Viewer", result.User.Role);
        Assert.Equal("hashed:secret123", _users.Users.Single().PasswordHash);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsDuplicateEmail()
    {
        _users.Seed("Ana", "Diaz", "ana@example.com", "Viewer");

        var result = await _service.CreateUserAsync(NewRequest("ANA@example.com"));

        Assert.Equal(SaveUserStatus.DuplicateEmail, result.Status);
        Assert.Single(_users.Users);
    }

    [Fact]
    public async Task RemoveUser_Missing_ReturnsNotFound()
    {
        var result = await _service.RemoveUserAsync(99, 1);

        Assert.Equal(DeleteUserResult.NotFound, result);
    }

    [Fact]
    public async Task RemoveUser_Self_ReturnsSelf()
    {
        var admin = _users.Seed("Ana", "Diaz", "ana@example.com", "Admin");
        _users.Seed("Luis", "Mora", "luis@example.com", "Admin");

        var result = await _service.RemoveUserAsync(admin.Id, admin.Id);

        Assert.Equal(DeleteUserResult.Self, result);
        Assert.Empty(_users.Removed);
    }

    [Fact]
    public async Task RemoveUser_LastAdmin_ReturnsLastAdmin()
    {
        var admin = _users.Seed("Ana", "Diaz", "ana@example.com", "Admin");
        var viewer = _users.Seed("Luis", "Mora", "luis@example.com", "Viewer");

        var result = await _service.RemoveUserAsync(admin.Id, viewer.Id);

        Assert.Equal(DeleteUserResult.LastAdmin, result);
        Assert.Empty(_users.Removed);
    }

    [Fact]
    public async Task RemoveUser_AdminWithAnotherAdmin_Deletes()
    {
        var current = _users.Seed("Ana", "Diaz", "ana@example.com", "Admin");
        var other = _users.Seed("Luis", "Mora", "luis@example.com", "Admin");

        var result = await _service.RemoveUserAsync(other.Id, current.Id);

        Assert.Equal(DeleteUserResult.Deleted, result);
        Assert.Single(_users.Removed);
    }

    private static UserRequest NewRequest(string email)
    {
        var request = new UserRequest()
        {
            Name = "Ana",
            LastName = "Diaz",
            Email = email,
            Password = "secret123",
            Role = "Viewer"
        };

        return request;
    }

    private class StubPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string passwordHash, string password)
        {
            return passwordHash == $"hashed:{password}";
        }
    }
}
