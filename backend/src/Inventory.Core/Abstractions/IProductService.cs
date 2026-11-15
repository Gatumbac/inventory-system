using Inventory.Core.Dtos;

namespace Inventory.Core.Abstractions;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetProductsAsync();
    Task<ProductResponse?> GetProductAsync(int id);
    Task<ProductResponse> CreateProductAsync(ProductRequest productRequest);
    Task<ProductResponse?> UpdateProductAsync(int id, ProductRequest productRequest);
}
