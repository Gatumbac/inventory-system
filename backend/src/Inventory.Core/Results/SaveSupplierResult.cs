using Inventory.Core.Dtos;

namespace Inventory.Core.Results;

public enum SaveSupplierStatus
{
    Saved,
    NotFound,
    DuplicateName
}

public class SaveSupplierResult
{
    public required SaveSupplierStatus Status { get; set; }
    public SupplierResponse? Supplier { get; set; }
}
