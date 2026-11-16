using Inventory.Core.Dtos;

namespace Inventory.Core.Abstractions;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<MeResponse?> GetCurrentUserAsync(int userId);
}
