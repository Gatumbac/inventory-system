using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(DbContext context, IConfiguration configuration, IPasswordHasher hasher)
    {
        var email = configuration["Seed:AdminEmail"]
            ?? throw new InvalidOperationException("Setting 'Seed:AdminEmail' not found.");
        var password = configuration["Seed:AdminPassword"]
            ?? throw new InvalidOperationException("Setting 'Seed:AdminPassword' not found.");

        var users = context.Set<User>();
        if (users.Any(u => u.Email == email))
        {
            return;
        }

        var adminRoleId = context.Set<Role>().Single(r => r.Name == RoleNames.Admin).Id;

        users.Add(new User
        {
            Name = "Admin",
            LastName = "System",
            Email = email,
            PasswordHash = hasher.Hash(password),
            RoleId = adminRoleId
        });
        context.SaveChanges();
    }
}
