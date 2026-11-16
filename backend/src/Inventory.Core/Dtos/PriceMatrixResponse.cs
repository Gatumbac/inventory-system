using Inventory.Core.Results;

namespace Inventory.Core.Dtos;

public class PriceMatrixResponse
{
    public required List<PriceMatrixSupplierResponse> Suppliers { get; set; }
    public required PagedResult<PriceMatrixRowResponse> Products { get; set; }
}
