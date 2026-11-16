using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Supplier>> GetPagedAsync(PageRequest pageRequest)
    {
        var suppliers = _context.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ThenBy(s => s.Id);
        return await PagedQuery.ToPagedAsync(suppliers, pageRequest);
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Supplier?> GetByIdForUpdateAsync(int id)
    {
        return await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Suppliers.AnyAsync(s => s.Id == id);
    }

    public async Task AddAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Supplier supplier)
    {
        supplier.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        return await _context.Suppliers.AnyAsync(s => s.Name == name && s.Id != excludeId);
    }

    public async Task<bool> HasProductSuppliersAsync(int supplierId)
    {
        return await _context.ProductSuppliers.AnyAsync(ps => ps.SupplierId == supplierId);
    }
}
