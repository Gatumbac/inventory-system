namespace Inventory.Core.Dtos;

public class PriceMatrixPriceResponse
{
    public required int SupplierId { get; set; }
    public required decimal Price { get; set; }
}
