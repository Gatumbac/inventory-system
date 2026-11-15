using Inventory.Core.Abstractions;
using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class ProductSupplierRepository : IProductSupplierRepository
{
    private readonly AppDbContext _context;

    public ProductSupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductSupplier>> GetByProductAsync(int productId)
    {
        return await _context.ProductSuppliers
            .AsNoTracking()
            .Include(ps => ps.Supplier)
            .Where(ps => ps.ProductId == productId)
            .OrderBy(ps => ps.Supplier.Name)
            .ThenBy(ps => ps.LotNumber)
            .ToListAsync();
    }

    public async Task<ProductSupplier?> GetByIdAsync(int productId, int id)
    {
        return await _context.ProductSuppliers
            .AsNoTracking()
            .Include(ps => ps.Supplier)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.Id == id);
    }

    public async Task<ProductSupplier?> GetByIdForUpdateAsync(int productId, int id)
    {
        return await _context.ProductSuppliers
            .Include(ps => ps.Supplier)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.Id == id);
    }

    public async Task AddAsync(ProductSupplier productSupplier)
    {
        _context.ProductSuppliers.Add(productSupplier);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(ProductSupplier productSupplier)
    {
        productSupplier.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> LotExistsAsync(int productId, int supplierId, string? lotNumber, int? excludeId = null)
    {
        return await _context.ProductSuppliers.AnyAsync(ps =>
            ps.ProductId == productId
            && ps.SupplierId == supplierId
            && ps.LotNumber == lotNumber
            && ps.Id != excludeId);
    }
}
