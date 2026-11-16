namespace Inventory.Core.Dtos;

public class PriceMatrixRowResponse
{
    public required int ProductId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public List<PriceMatrixPriceResponse> Prices { get; set; } = new List<PriceMatrixPriceResponse>();
}
