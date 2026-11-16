using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;

namespace Inventory.Core.Services;

public class DashboardService : IDashboardService
{
    public const int LowStockThreshold = 5;
    public const int LowStockLimit = 5;

    private readonly IDashboardRepository _dashboard;

    public DashboardService(IDashboardRepository dashboard)
    {
        _dashboard = dashboard;
    }

    public async Task<DashboardResponse> GetDashboardAsync()
    {
        var lowStock = await _dashboard.GetLowStockAsync(LowStockThreshold, LowStockLimit);

        var response = new DashboardResponse()
        {
            ProductCount = await _dashboard.CountProductsAsync(),
            SupplierCount = await _dashboard.CountSuppliersAsync(),
            TotalStock = await _dashboard.SumStockAsync(),
            InventoryValue = await _dashboard.SumInventoryValueAsync(),
            LowStock = lowStock.Select(ps => ToLowStockItem(ps)).ToList()
        };

        return response;
    }

    private static LowStockItemResponse ToLowStockItem(ProductSupplier productSupplier)
    {
        var item = new LowStockItemResponse()
        {
            ProductId = productSupplier.ProductId,
            ProductName = productSupplier.Product.Name,
            SupplierName = productSupplier.Supplier.Name,
            LotNumber = productSupplier.LotNumber,
            Stock = productSupplier.Stock
        };

        return item;
    }
}
