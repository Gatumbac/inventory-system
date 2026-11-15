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

    public async Task<ProductResponse> CreateProductAsync(ProductRequest productRequest)
    {
        var product = ToProduct(productRequest);
        await _products.AddAsync(product);
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

    private Product ToProduct(ProductRequest productRequest)
    {
        var attributes = productRequest.Attributes.Select(pa => ToProductAttribute(pa)).ToList();
        var product = new Product()
        {
            Name = productRequest.Name.Trim(),
            Description = productRequest.Description?.Trim(),
            Attributes = attributes
        };
        return product;
    }

    private ProductAttribute ToProductAttribute(ProductAttributeRequest productAttributeRequest)
    {
        var productAttribute = new ProductAttribute()
        {
            Name = productAttributeRequest.Name.Trim(),
            Value = productAttributeRequest.Value.Trim()
        };
        return productAttribute;
    }
}
