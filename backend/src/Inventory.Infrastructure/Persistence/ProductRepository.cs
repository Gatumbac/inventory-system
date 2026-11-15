using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Product>> GetPagedAsync(ProductFilterRequest filterRequest)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Attributes);

        var products = ApplySearch(query, filterRequest.Search)
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id);

        return await PagedQuery.ToPagedAsync(products, filterRequest);
    }

    private static IQueryable<Product> ApplySearch(IQueryable<Product> query, string? search)
    {
        var term = search?.Trim();
        if (string.IsNullOrEmpty(term))
        {
            return query;
        }

        return query.Where(p => p.Code.Contains(term) || p.Name.Contains(term));
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Product product)
    {
        product.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        return await _context.Products.AnyAsync(p => p.Code == code && p.Id != excludeId);
    }

    public async Task<bool> HasSuppliersAsync(int productId)
    {
        return await _context.ProductSuppliers.AnyAsync(ps => ps.ProductId == productId);
    }
}
