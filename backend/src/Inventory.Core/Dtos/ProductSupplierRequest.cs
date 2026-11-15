using System.ComponentModel.DataAnnotations;
using Inventory.Core.Constants;

namespace Inventory.Core.Dtos;

public class ProductSupplierRequest
{
    [StringLength(50)]
    public string? LotNumber { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99", ErrorMessage = ErrorMessages.ProductSupplierPriceInvalid)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.ProductSupplierStockInvalid)]
    public int Stock { get; set; }
}
