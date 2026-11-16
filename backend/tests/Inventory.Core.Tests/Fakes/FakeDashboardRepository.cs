using Inventory.Core.Abstractions;
using Inventory.Core.Entities;

namespace Inventory.Core.Tests.Fakes;

public class FakeDashboardRepository : IDashboardRepository
{
    public int ProductCount { get; set; }
    public int SupplierCount { get; set; }
    public int TotalStock { get; set; }
    public decimal InventoryValue { get; set; }
    public List<ProductSupplier> LowStock { get; } = new List<ProductSupplier>();
    public int? RequestedThreshold { get; private set; }
    public int? RequestedLimit { get; private set; }

    public Task<int> CountProductsAsync()
    {
        return Task.FromResult(ProductCount);
    }

    public Task<int> CountSuppliersAsync()
    {
        return Task.FromResult(SupplierCount);
    }

    public Task<int> SumStockAsync()
    {
        return Task.FromResult(TotalStock);
    }

    public Task<decimal> SumInventoryValueAsync()
    {
        return Task.FromResult(InventoryValue);
    }

    public Task<List<ProductSupplier>> GetLowStockAsync(int threshold, int limit)
    {
        RequestedThreshold = threshold;
        RequestedLimit = limit;
        return Task.FromResult(LowStock.ToList());
    }
}
