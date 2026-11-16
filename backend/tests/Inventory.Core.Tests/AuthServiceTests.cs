using Inventory.Core.Abstractions;
using Inventory.Core.Entities;
using Inventory.Core.Services;
using Inventory.Core.Tests.Fakes;

namespace Inventory.Core.Tests;

public class AuthServiceTests
{
    private readonly FakeUserRepository _users = new FakeUserRepository();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_users, new StubPasswordHasher(), new StubTokenService());
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsMappedUser()
    {
        var user = _users.Seed("Ada", "Lovelace", "ada@example.com", "Admin");

        var result = await _service.GetCurrentUserAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Ada", result.Name);
        Assert.Equal("Lovelace", result.LastName);
        Assert.Equal("ada@example.com", result.Email);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsNullWhenUserDoesNotExist()
    {
        var result = await _service.GetCurrentUserAsync(99);

        Assert.Null(result);
    }

    private class StubPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return password;
        }

        public bool Verify(string passwordHash, string password)
        {
            return passwordHash == password;
        }
    }

    private class StubTokenService : ITokenService
    {
        public string GenerateToken(User user)
        {
            return "token";
        }
    }
}
