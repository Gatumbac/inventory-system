using Inventory.Core.Entities;
using Inventory.Core.Services;
using Inventory.Core.Tests.Fakes;

namespace Inventory.Core.Tests;

public class DashboardServiceTests
{
    private readonly FakeDashboardRepository _dashboard = new FakeDashboardRepository();
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _service = new DashboardService(_dashboard);
    }

    [Fact]
    public async Task GetDashboard_MapsTotalsAndLowStock()
    {
        _dashboard.ProductCount = 6;
        _dashboard.SupplierCount = 5;
        _dashboard.TotalStock = 120;
        _dashboard.InventoryValue = 4500.50m;
        _dashboard.LowStock.Add(NewProductSupplier());

        var result = await _service.GetDashboardAsync();

        Assert.Equal(6, result.ProductCount);
        Assert.Equal(5, result.SupplierCount);
        Assert.Equal(120, result.TotalStock);
        Assert.Equal(4500.50m, result.InventoryValue);
        var item = Assert.Single(result.LowStock);
        Assert.Equal(7, item.ProductId);
        Assert.Equal("Laptop", item.ProductName);
        Assert.Equal("Proveedor A", item.SupplierName);
        Assert.Null(item.LotNumber);
        Assert.Equal(2, item.Stock);
    }

    [Fact]
    public async Task GetDashboard_RequestsLowStockWithThresholdAndLimit()
    {
        await _service.GetDashboardAsync();

        Assert.Equal(DashboardService.LowStockThreshold, _dashboard.RequestedThreshold);
        Assert.Equal(DashboardService.LowStockLimit, _dashboard.RequestedLimit);
    }

    private static ProductSupplier NewProductSupplier()
    {
        var productSupplier = new ProductSupplier()
        {
            ProductId = 7,
            Stock = 2,
            Product = new Product()
            {
                Code = "LAP-1",
                Name = "Laptop"
            },
            Supplier = new Supplier()
            {
                Name = "Proveedor A"
            }
        };

        return productSupplier;
    }
}
