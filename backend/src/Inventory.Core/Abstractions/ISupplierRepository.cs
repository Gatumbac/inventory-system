using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> GetAllAsync();
    Task<bool> ExistsAsync(int id);
}
