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

    public async Task<PagedResult<ProductResponse>> GetProductsAsync(ProductFilterRequest filterRequest)
    {
        var page = await _products.GetPagedAsync(filterRequest);

        return page.Map(p => ToProductResponse(p));
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
            return ToSaveResult(SaveProductStatus.DuplicateCode);
        }

        await _products.AddAsync(product);
        return ToSaveResult(SaveProductStatus.Saved, ToProductResponse(product));
    }

    public async Task<SaveProductResult> UpdateProductAsync(int id, ProductRequest productRequest)
    {
        var product = await _products.GetByIdForUpdateAsync(id);
        if (product == null)
        {
            return ToSaveResult(SaveProductStatus.NotFound);
        }

        var code = NormalizeCode(productRequest.Code);
        if (await _products.CodeExistsAsync(code, id))
        {
            return ToSaveResult(SaveProductStatus.DuplicateCode);
        }

        ApplyRequest(product, code, productRequest);
        await _products.SaveChangesAsync();
        return ToSaveResult(SaveProductStatus.Saved, ToProductResponse(product));
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

    private static SaveProductResult ToSaveResult(SaveProductStatus status, ProductResponse? product = null)
    {
        var result = new SaveProductResult()
        {
            Status = status,
            Product = product
        };

        return result;
    }

    private void ApplyRequest(Product product, string code, ProductRequest productRequest)
    {
        product.Code = code;
        product.Name = productRequest.Name.Trim();
        product.Description = productRequest.Description?.Trim();
        product.Attributes.Clear();
        foreach (var attribute in productRequest.Attributes)
        {
            product.Attributes.Add(ToProductAttribute(attribute));
        }
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
