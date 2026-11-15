using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Dtos;

public class CreateProductSupplierRequest : ProductSupplierRequest
{
    [Range(1, int.MaxValue)]
    public int SupplierId { get; set; }
}
