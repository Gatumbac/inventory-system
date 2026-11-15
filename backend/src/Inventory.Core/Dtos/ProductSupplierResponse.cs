namespace Inventory.Core.Dtos;

public class ProductSupplierResponse
{
    public required int Id { get; set; }
    public required int ProductId { get; set; }
    public required int SupplierId { get; set; }
    public required string SupplierName { get; set; }
    public string? LotNumber { get; set; }
    public required decimal Price { get; set; }
    public required int Stock { get; set; }
}
