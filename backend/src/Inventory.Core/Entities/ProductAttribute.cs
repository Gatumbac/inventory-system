namespace Inventory.Core.Entities;

public class ProductAttribute
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public Product Product { get; set; } = null!;
}
