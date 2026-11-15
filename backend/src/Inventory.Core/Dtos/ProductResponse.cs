namespace Inventory.Core.Dtos;

public class ProductResponse
{
    public required int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<ProductAttributeResponse> Attributes { get; set; } = new List<ProductAttributeResponse>();
}
