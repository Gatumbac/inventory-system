using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier?> GetByIdForUpdateAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Supplier supplier);
    Task SaveChangesAsync();
    Task RemoveAsync(Supplier supplier);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<bool> HasProductSuppliersAsync(int supplierId);
}
