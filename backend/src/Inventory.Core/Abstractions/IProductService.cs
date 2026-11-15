using Inventory.Core.Dtos;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetProductsAsync();
    Task<ProductResponse?> GetProductAsync(int id);
    Task<SaveProductResult> CreateProductAsync(ProductRequest productRequest);
    Task<SaveProductResult> UpdateProductAsync(int id, ProductRequest productRequest);
    Task<DeleteProductResult> RemoveProductAsync(int id);
}
