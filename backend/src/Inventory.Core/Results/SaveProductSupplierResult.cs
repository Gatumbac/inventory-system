using Inventory.Core.Dtos;

namespace Inventory.Core.Results;

public enum SaveProductSupplierStatus
{
    Saved,
    ProductNotFound,
    SupplierNotFound,
    NotFound,
    DuplicateLot
}

public class SaveProductSupplierResult
{
    public required SaveProductSupplierStatus Status { get; set; }
    public ProductSupplierResponse? ProductSupplier { get; set; }
}
