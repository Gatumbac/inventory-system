using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface IDashboardRepository
{
    Task<int> CountProductsAsync();
    Task<int> CountSuppliersAsync();
    Task<int> SumStockAsync();
    Task<decimal> SumInventoryValueAsync();
    Task<List<ProductSupplier>> GetLowStockAsync(int threshold, int limit);
}
