using Inventory.Core.Dtos;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierResponse>> GetSuppliersAsync();
    Task<SupplierResponse?> GetSupplierAsync(int id);
    Task<SaveSupplierResult> CreateSupplierAsync(SupplierRequest supplierRequest);
    Task<SaveSupplierResult> UpdateSupplierAsync(int id, SupplierRequest supplierRequest);
    Task<DeleteSupplierResult> RemoveSupplierAsync(int id);
}
