using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;

namespace Inventory.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetProductsAsync()
    {
        var products = await _products.GetAllAsync();

        return products.Select(p => ToProductResponse(p)).ToList();
    }

    public async Task<ProductResponse?> GetProductAsync(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        return ToProductResponse(product);
    }

    private ProductResponse ToProductResponse(Product product)
    {
        var attributes = product.Attributes.Select(a => ToProductAttributeResponse(a)).ToList();

        var response = new ProductResponse()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Attributes = attributes
        };

        return response;
    }

    private ProductAttributeResponse ToProductAttributeResponse(ProductAttribute attribute)
    {
        var response = new ProductAttributeResponse()
        {
            Name = attribute.Name,
            Value = attribute.Value
        };

        return response;
    }
}
