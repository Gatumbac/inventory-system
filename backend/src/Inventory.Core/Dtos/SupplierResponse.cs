namespace Inventory.Core.Dtos;

public class SupplierResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
