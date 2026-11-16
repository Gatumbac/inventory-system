namespace Inventory.Core.Dtos;

public class DashboardResponse
{
    public required int ProductCount { get; set; }
    public required int SupplierCount { get; set; }
    public required int TotalStock { get; set; }
    public required decimal InventoryValue { get; set; }
    public List<LowStockItemResponse> LowStock { get; set; } = new List<LowStockItemResponse>();
}
