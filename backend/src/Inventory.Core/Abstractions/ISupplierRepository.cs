using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface ISupplierRepository
{
    Task<PagedResult<Supplier>> GetPagedAsync(PageRequest pageRequest);
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier?> GetByIdForUpdateAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Supplier supplier);
    Task SaveChangesAsync();
    Task RemoveAsync(Supplier supplier);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<bool> HasProductSuppliersAsync(int supplierId);
}
