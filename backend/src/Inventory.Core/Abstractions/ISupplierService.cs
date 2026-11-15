using Inventory.Core.Dtos;

namespace Inventory.Core.Abstractions;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierResponse>> GetSuppliersAsync();
}
