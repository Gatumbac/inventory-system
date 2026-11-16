using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Tests.Fakes;

public class FakeSupplierRepository : ISupplierRepository
{
    private int _nextId = 1;

    public List<Supplier> Suppliers { get; } = new List<Supplier>();
    public HashSet<int> SupplierIdsWithProductSuppliers { get; } = new HashSet<int>();
    public int SaveChangesCount { get; private set; }
    public List<Supplier> Removed { get; } = new List<Supplier>();

    public Supplier Seed(string name)
    {
        var supplier = new Supplier()
        {
            Id = _nextId++,
            Name = name
        };

        Suppliers.Add(supplier);
        return supplier;
    }

    public Task<PagedResult<Supplier>> GetPagedAsync(SupplierFilterRequest filterRequest)
    {
        var page = new PagedResult<Supplier>()
        {
            Items = Suppliers.ToList(),
            Page = filterRequest.Page,
            PageSize = filterRequest.PageSize,
            TotalCount = Suppliers.Count
        };

        return Task.FromResult(page);
    }

    public Task<Supplier?> GetByIdAsync(int id)
    {
        return Task.FromResult(Suppliers.FirstOrDefault(s => s.Id == id));
    }

    public Task<Supplier?> GetByIdForUpdateAsync(int id)
    {
        return GetByIdAsync(id);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(Suppliers.Any(s => s.Id == id));
    }

    public Task AddAsync(Supplier supplier)
    {
        supplier.Id = _nextId++;
        Suppliers.Add(supplier);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        SaveChangesCount++;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Supplier supplier)
    {
        Suppliers.Remove(supplier);
        Removed.Add(supplier);
        return Task.CompletedTask;
    }

    public Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var exists = Suppliers.Any(s => s.Name == name && s.Id != excludeId);
        return Task.FromResult(exists);
    }

    public Task<bool> HasProductSuppliersAsync(int supplierId)
    {
        return Task.FromResult(SupplierIdsWithProductSuppliers.Contains(supplierId));
    }
}
