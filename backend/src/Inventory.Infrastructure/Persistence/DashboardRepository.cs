using Inventory.Core.Abstractions;
using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountProductsAsync()
    {
        return await _context.Products.CountAsync();
    }

    public async Task<int> CountSuppliersAsync()
    {
        return await _context.Suppliers.CountAsync();
    }

    public async Task<int> SumStockAsync()
    {
        return await _context.ProductSuppliers.SumAsync(ps => ps.Stock);
    }

    public async Task<decimal> SumInventoryValueAsync()
    {
        return await _context.ProductSuppliers.SumAsync(ps => ps.Price * ps.Stock);
    }

    public async Task<List<ProductSupplier>> GetLowStockAsync(int threshold, int limit)
    {
        return await _context.ProductSuppliers
            .AsNoTracking()
            .Include(ps => ps.Product)
            .Include(ps => ps.Supplier)
            .Where(ps => ps.Stock <= threshold)
            .OrderBy(ps => ps.Stock)
            .ThenBy(ps => ps.Product.Name)
            .ThenBy(ps => ps.Id)
            .Take(limit)
            .ToListAsync();
    }
}
