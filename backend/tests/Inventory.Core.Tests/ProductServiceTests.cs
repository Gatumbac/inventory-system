using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Inventory.Core.Services;
using Inventory.Core.Tests.Fakes;

namespace Inventory.Core.Tests;

public class ProductServiceTests
{
    private readonly FakeProductRepository _products = new FakeProductRepository();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_products);
    }

    [Fact]
    public async Task CreateProduct_NormalizesCodeAndSaves()
    {
        var result = await _service.CreateProductAsync(NewRequest("  ab-1 "));

        Assert.Equal(SaveProductStatus.Saved, result.Status);
        Assert.Equal("AB-1", result.Product!.Code);
        Assert.Single(_products.Products);
    }

    [Fact]
    public async Task CreateProduct_DuplicateCode_ReturnsDuplicateCode()
    {
        _products.Seed("AB-1");

        var result = await _service.CreateProductAsync(NewRequest("ab-1"));

        Assert.Equal(SaveProductStatus.DuplicateCode, result.Status);
        Assert.Single(_products.Products);
    }

    [Fact]
    public async Task UpdateProduct_Missing_ReturnsNotFound()
    {
        var result = await _service.UpdateProductAsync(99, NewRequest("AB-1"));

        Assert.Equal(SaveProductStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateProduct_CodeOfAnotherProduct_ReturnsDuplicateCode()
    {
        _products.Seed("AB-1");
        var other = _products.Seed("AB-2");

        var result = await _service.UpdateProductAsync(other.Id, NewRequest("ab-1"));

        Assert.Equal(SaveProductStatus.DuplicateCode, result.Status);
        Assert.Equal(0, _products.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateProduct_KeepingOwnCode_Saves()
    {
        var product = _products.Seed("AB-1");

        var result = await _service.UpdateProductAsync(product.Id, NewRequest("ab-1"));

        Assert.Equal(SaveProductStatus.Saved, result.Status);
        Assert.Equal(1, _products.SaveChangesCount);
    }

    [Fact]
    public async Task RemoveProduct_Missing_ReturnsNotFound()
    {
        var result = await _service.RemoveProductAsync(99);

        Assert.Equal(DeleteProductResult.NotFound, result);
    }

    [Fact]
    public async Task RemoveProduct_WithSuppliers_ReturnsHasSuppliersAndKeepsProduct()
    {
        var product = _products.Seed("AB-1");
        _products.ProductIdsWithSuppliers.Add(product.Id);

        var result = await _service.RemoveProductAsync(product.Id);

        Assert.Equal(DeleteProductResult.HasSuppliers, result);
        Assert.Empty(_products.Removed);
    }

    [Fact]
    public async Task RemoveProduct_WithoutSuppliers_Deletes()
    {
        var product = _products.Seed("AB-1");

        var result = await _service.RemoveProductAsync(product.Id);

        Assert.Equal(DeleteProductResult.Deleted, result);
        Assert.Single(_products.Removed);
    }

    private static ProductRequest NewRequest(string code)
    {
        var request = new ProductRequest()
        {
            Code = code,
            Name = "Product"
        };

        return request;
    }
}
