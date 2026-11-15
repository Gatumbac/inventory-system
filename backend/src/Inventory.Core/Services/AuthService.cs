using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;

namespace Inventory.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IUserRepository users, IPasswordHasher hasher, ITokenService tokens)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return null;
        }

        if (!_hasher.Verify(user.PasswordHash, request.Password))
        {
            return null;
        }

        var response = new LoginResponse()
        {
            Token = _tokens.GenerateToken(user),
            Role = user.Role.Name
        };
        return response;
    }
}
