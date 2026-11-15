using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetPagedAsync(ProductFilterRequest filterRequest);
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task<Product?> GetByIdForUpdateAsync(int id);
    Task SaveChangesAsync();
    Task RemoveAsync(Product product);
    Task<bool> HasSuppliersAsync(int productId);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}
