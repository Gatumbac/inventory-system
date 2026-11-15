using Inventory.Core.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Infrastructure.Security;

public class IdentityPasswordHasher : IPasswordHasher
{
    private static readonly object Subject = new object();
    private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

    public string Hash(string password)
    {
        return _hasher.HashPassword(Subject, password);
    }

    public bool Verify(string passwordHash, string password)
    {
        var result = _hasher.VerifyHashedPassword(Subject, passwordHash, password);
        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
