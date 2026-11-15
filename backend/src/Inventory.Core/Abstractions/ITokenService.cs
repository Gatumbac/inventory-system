using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface ITokenService
{
    string GenerateToken(User user);
}
