namespace Inventory.Core.Entities;

public class ProductSupplier
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SupplierId { get; set; }
    public string? LotNumber { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Product Product { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
}
