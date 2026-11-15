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
        var adminEmail = GetSetting(configuration, "Seed:AdminEmail");
        var adminPassword = GetSetting(configuration, "Seed:AdminPassword");
        var viewerEmail = GetSetting(configuration, "Seed:ViewerEmail");
        var viewerPassword = GetSetting(configuration, "Seed:ViewerPassword");

        EnsureUser(context, hasher, "Admin", "System", adminEmail, adminPassword, RoleNames.Admin);
        EnsureUser(context, hasher, "Viewer", "System", viewerEmail, viewerPassword, RoleNames.Viewer);
    }

    private static string GetSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (value == null)
        {
            throw new InvalidOperationException("Setting '" + key + "' not found.");
        }

        return value;
    }

    private static void EnsureUser(
        DbContext context,
        IPasswordHasher hasher,
        string name,
        string lastName,
        string email,
        string password,
        string roleName)
    {
        var users = context.Set<User>();
        if (users.Any(u => u.Email == email))
        {
            return;
        }

        var roleId = context.Set<Role>().Single(r => r.Name == roleName).Id;

        var user = new User
        {
            Name = name,
            LastName = lastName,
            Email = email,
            PasswordHash = hasher.Hash(password),
            RoleId = roleId
        };

        users.Add(user);
        context.SaveChanges();
    }
}
