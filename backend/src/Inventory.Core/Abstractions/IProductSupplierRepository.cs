using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface IProductSupplierRepository
{
    Task<IReadOnlyList<ProductSupplier>> GetByProductAsync(int productId);
    Task<ProductSupplier?> GetByIdAsync(int productId, int id);
    Task<ProductSupplier?> GetByIdForUpdateAsync(int productId, int id);
    Task AddAsync(ProductSupplier productSupplier);
    Task SaveChangesAsync();
    Task RemoveAsync(ProductSupplier productSupplier);
    Task<bool> LotExistsAsync(int productId, int supplierId, string? lotNumber, int? excludeId = null);
}
