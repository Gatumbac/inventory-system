using Inventory.Core.Entities;

namespace Inventory.Core.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task<Product?> GetByIdForUpdateAsync(int id);
    Task SaveChangesAsync();
}
