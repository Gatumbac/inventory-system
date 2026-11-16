using Inventory.Core.Abstractions;
using Inventory.Core.Constants;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<PagedResult<User>> GetPagedAsync(UserFilterRequest filterRequest)
    {
        var query = filterRequest.Inactive
            ? _context.Users.IgnoreQueryFilters().Where(u => u.DeletedAt != null)
            : _context.Users;

        var users = ApplySearch(query.AsNoTracking().Include(u => u.Role), filterRequest.Search)
            .OrderBy(u => u.Name)
            .ThenBy(u => u.LastName)
            .ThenBy(u => u.Id);

        return await PagedQuery.ToPagedAsync(users, filterRequest);
    }

    private static IQueryable<User> ApplySearch(IQueryable<User> query, string? search)
    {
        var term = search?.Trim();
        if (string.IsNullOrEmpty(term))
        {
            return query;
        }

        return query.Where(u => u.Name.Contains(term) || u.LastName.Contains(term) || u.Email.Contains(term));
    }

    public async Task<User?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<Role> GetRoleByNameAsync(string name)
    {
        return await _context.Roles.SingleAsync(r => r.Name == name);
    }

    public async Task<int> CountAdminsAsync()
    {
        return await _context.Users.CountAsync(u => u.Role.Name == RoleNames.Admin);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(User user)
    {
        user.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetInactiveByIdForUpdateAsync(int id)
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt != null);
    }

    public async Task ReactivateAsync(User user)
    {
        user.DeletedAt = null;
        await _context.SaveChangesAsync();
    }
}
