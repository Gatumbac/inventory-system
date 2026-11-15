using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
}
