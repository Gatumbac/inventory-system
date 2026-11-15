using Inventory.Core.Dtos;

namespace Inventory.Core.Results;

public enum SaveProductStatus
{
    Saved,
    NotFound,
    DuplicateCode
}

public class SaveProductResult
{
    public required SaveProductStatus Status { get; set; }
    public ProductResponse? Product { get; set; }
}
