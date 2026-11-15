using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

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

    public async Task<SaveProductResult> CreateProductAsync(ProductRequest productRequest)
    {
        var product = ToProduct(productRequest);
        if (await _products.CodeExistsAsync(product.Code))
        {
            var duplicateResult = new SaveProductResult()
            {
                Status = SaveProductStatus.DuplicateCode
            };

            return duplicateResult;
        }

        await _products.AddAsync(product);
        var result = new SaveProductResult()
        {
            Status = SaveProductStatus.Saved,
            Product = ToProductResponse(product)
        };

        return result;
    }

    public async Task<SaveProductResult> UpdateProductAsync(int id, ProductRequest productRequest)
    {
        var product = await _products.GetByIdForUpdateAsync(id);
        if (product == null)
        {
            var notFoundResult = new SaveProductResult()
            {
                Status = SaveProductStatus.NotFound
            };

            return notFoundResult;
        }

        var code = NormalizeCode(productRequest.Code);
        if (await _products.CodeExistsAsync(code, id))
        {
            var duplicateResult = new SaveProductResult()
            {
                Status = SaveProductStatus.DuplicateCode
            };

            return duplicateResult;
        }

        product.Code = code;
        product.Name = productRequest.Name.Trim();
        product.Description = productRequest.Description?.Trim();
        product.Attributes.Clear();
        foreach (var attribute in productRequest.Attributes)
        {
            product.Attributes.Add(ToProductAttribute(attribute));
        }

        await _products.SaveChangesAsync();
        var result = new SaveProductResult()
        {
            Status = SaveProductStatus.Saved,
            Product = ToProductResponse(product)
        };

        return result;
    }

    public async Task<DeleteProductResult> RemoveProductAsync(int id)
    {
        var product = await _products.GetByIdForUpdateAsync(id);
        if (product == null)
        {
            return DeleteProductResult.NotFound;
        }

        if (await _products.HasSuppliersAsync(id))
        {
            return DeleteProductResult.HasSuppliers;
        }

        await _products.RemoveAsync(product);
        return DeleteProductResult.Deleted;
    }

    private ProductResponse ToProductResponse(Product product)
    {
        var attributes = product.Attributes.Select(a => ToProductAttributeResponse(a)).ToList();

        var response = new ProductResponse()
        {
            Id = product.Id,
            Code = product.Code,
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
            Code = NormalizeCode(productRequest.Code),
            Name = productRequest.Name.Trim(),
            Description = productRequest.Description?.Trim(),
            Attributes = attributes
        };
        return product;
    }

    private static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
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
