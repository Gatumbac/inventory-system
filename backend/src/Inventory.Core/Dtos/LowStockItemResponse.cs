namespace Inventory.Core.Dtos;

public class LowStockItemResponse
{
    public required int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string SupplierName { get; set; }
    public string? LotNumber { get; set; }
    public required int Stock { get; set; }
}
