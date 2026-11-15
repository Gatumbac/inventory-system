using Inventory.Core.Abstractions;
using Inventory.Core.Entities;

namespace Inventory.Core.Tests.Fakes;

public class FakeProductSupplierRepository : IProductSupplierRepository
{
    private int _nextId = 1;

    public List<ProductSupplier> ProductSuppliers { get; } = new List<ProductSupplier>();
    public int SaveChangesCount { get; private set; }
    public List<ProductSupplier> Removed { get; } = new List<ProductSupplier>();

    public ProductSupplier Seed(int productId, int supplierId, string? lotNumber)
    {
        var productSupplier = new ProductSupplier()
        {
            Id = _nextId++,
            ProductId = productId,
            SupplierId = supplierId,
            LotNumber = lotNumber,
            Supplier = NewSupplier(supplierId)
        };

        ProductSuppliers.Add(productSupplier);
        return productSupplier;
    }

    public Task<IReadOnlyList<ProductSupplier>> GetByProductAsync(int productId)
    {
        IReadOnlyList<ProductSupplier> items = ProductSuppliers.Where(ps => ps.ProductId == productId).ToList();
        return Task.FromResult(items);
    }

    public Task<ProductSupplier?> GetByIdAsync(int productId, int id)
    {
        var item = ProductSuppliers.FirstOrDefault(ps => ps.ProductId == productId && ps.Id == id);
        return Task.FromResult(item);
    }

    public Task<ProductSupplier?> GetByIdForUpdateAsync(int productId, int id)
    {
        return GetByIdAsync(productId, id);
    }

    public Task AddAsync(ProductSupplier productSupplier)
    {
        productSupplier.Id = _nextId++;
        productSupplier.Supplier = NewSupplier(productSupplier.SupplierId);
        ProductSuppliers.Add(productSupplier);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        SaveChangesCount++;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(ProductSupplier productSupplier)
    {
        ProductSuppliers.Remove(productSupplier);
        Removed.Add(productSupplier);
        return Task.CompletedTask;
    }

    public Task<bool> LotExistsAsync(int productId, int supplierId, string? lotNumber, int? excludeId = null)
    {
        var exists = ProductSuppliers.Any(ps =>
            ps.ProductId == productId
            && ps.SupplierId == supplierId
            && ps.LotNumber == lotNumber
            && ps.Id != excludeId);

        return Task.FromResult(exists);
    }

    private static Supplier NewSupplier(int supplierId)
    {
        var supplier = new Supplier()
        {
            Id = supplierId,
            Name = $"Supplier {supplierId}"
        };

        return supplier;
    }
}
