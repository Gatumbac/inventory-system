using Inventory.Core.Dtos;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface IProductService
{
    Task<PagedResult<ProductResponse>> GetProductsAsync(ProductFilterRequest filterRequest);
    Task<ProductResponse?> GetProductAsync(int id);
    Task<SaveProductResult> CreateProductAsync(ProductRequest productRequest);
    Task<SaveProductResult> UpdateProductAsync(int id, ProductRequest productRequest);
    Task<DeleteProductResult> RemoveProductAsync(int id);
}
