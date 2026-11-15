using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Inventory.Core.Services;
using Inventory.Core.Tests.Fakes;

namespace Inventory.Core.Tests;

public class ProductSupplierServiceTests
{
    private readonly FakeProductSupplierRepository _productSuppliers = new FakeProductSupplierRepository();
    private readonly FakeProductRepository _products = new FakeProductRepository();
    private readonly FakeSupplierRepository _suppliers = new FakeSupplierRepository();
    private readonly ProductSupplierService _service;

    public ProductSupplierServiceTests()
    {
        _service = new ProductSupplierService(_productSuppliers, _products, _suppliers);
    }

    [Fact]
    public async Task Create_ProductMissing_ReturnsProductNotFound()
    {
        var supplier = _suppliers.Seed("Acme");

        var result = await _service.CreateProductSupplierAsync(99, NewRequest(supplier.Id, "L1"));

        Assert.Equal(SaveProductSupplierStatus.ProductNotFound, result.Status);
    }

    [Fact]
    public async Task Create_SupplierMissing_ReturnsSupplierNotFound()
    {
        var product = _products.Seed("AB-1");

        var result = await _service.CreateProductSupplierAsync(product.Id, NewRequest(99, "L1"));

        Assert.Equal(SaveProductSupplierStatus.SupplierNotFound, result.Status);
    }

    [Fact]
    public async Task Create_DuplicateLot_ReturnsDuplicateLot()
    {
        var product = _products.Seed("AB-1");
        var supplier = _suppliers.Seed("Acme");
        _productSuppliers.Seed(product.Id, supplier.Id, "L1");

        var result = await _service.CreateProductSupplierAsync(product.Id, NewRequest(supplier.Id, " L1 "));

        Assert.Equal(SaveProductSupplierStatus.DuplicateLot, result.Status);
        Assert.Single(_productSuppliers.ProductSuppliers);
    }

    [Fact]
    public async Task Create_SameLotForAnotherSupplier_Saves()
    {
        var product = _products.Seed("AB-1");
        var first = _suppliers.Seed("Acme");
        var second = _suppliers.Seed("Globex");
        _productSuppliers.Seed(product.Id, first.Id, "L1");

        var result = await _service.CreateProductSupplierAsync(product.Id, NewRequest(second.Id, "L1"));

        Assert.Equal(SaveProductSupplierStatus.Saved, result.Status);
        Assert.Equal(2, _productSuppliers.ProductSuppliers.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_BlankLot_IsStoredAsNullAndConflictsWithExistingNullLot(string? lot)
    {
        var product = _products.Seed("AB-1");
        var supplier = _suppliers.Seed("Acme");
        _productSuppliers.Seed(product.Id, supplier.Id, null);

        var result = await _service.CreateProductSupplierAsync(product.Id, NewRequest(supplier.Id, lot));

        Assert.Equal(SaveProductSupplierStatus.DuplicateLot, result.Status);
    }

    [Fact]
    public async Task Create_BlankLotWithoutExistingRow_SavesWithNullLot()
    {
        var product = _products.Seed("AB-1");
        var supplier = _suppliers.Seed("Acme");

        var result = await _service.CreateProductSupplierAsync(product.Id, NewRequest(supplier.Id, "  "));

        Assert.Equal(SaveProductSupplierStatus.Saved, result.Status);
        Assert.Null(result.ProductSupplier!.LotNumber);
    }

    [Fact]
    public async Task Update_Missing_ReturnsNotFound()
    {
        var result = await _service.UpdateProductSupplierAsync(1, 99, NewUpdate("L1"));

        Assert.Equal(SaveProductSupplierStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Update_LotOfAnotherRowOfSameSupplier_ReturnsDuplicateLot()
    {
        var product = _products.Seed("AB-1");
        var supplier = _suppliers.Seed("Acme");
        _productSuppliers.Seed(product.Id, supplier.Id, "L1");
        var other = _productSuppliers.Seed(product.Id, supplier.Id, "L2");

        var result = await _service.UpdateProductSupplierAsync(product.Id, other.Id, NewUpdate("L1"));

        Assert.Equal(SaveProductSupplierStatus.DuplicateLot, result.Status);
        Assert.Equal(0, _productSuppliers.SaveChangesCount);
    }

    [Fact]
    public async Task Update_KeepingOwnLot_Saves()
    {
        var product = _products.Seed("AB-1");
        var supplier = _suppliers.Seed("Acme");
        var row = _productSuppliers.Seed(product.Id, supplier.Id, "L1");

        var result = await _service.UpdateProductSupplierAsync(product.Id, row.Id, NewUpdate("L1"));

        Assert.Equal(SaveProductSupplierStatus.Saved, result.Status);
        Assert.Equal(1, _productSuppliers.SaveChangesCount);
    }

    [Fact]
    public async Task Remove_Missing_ReturnsNotFound()
    {
        var result = await _service.RemoveProductSupplierAsync(1, 99);

        Assert.Equal(DeleteProductSupplierResult.NotFound, result);
    }

    [Fact]
    public async Task Remove_Existing_Deletes()
    {
        var row = _productSuppliers.Seed(1, 1, "L1");

        var result = await _service.RemoveProductSupplierAsync(1, row.Id);

        Assert.Equal(DeleteProductSupplierResult.Deleted, result);
        Assert.Single(_productSuppliers.Removed);
    }

    private static CreateProductSupplierRequest NewRequest(int supplierId, string? lot)
    {
        var request = new CreateProductSupplierRequest()
        {
            SupplierId = supplierId,
            LotNumber = lot,
            Price = 10m,
            Stock = 5
        };

        return request;
    }

    private static ProductSupplierRequest NewUpdate(string? lot)
    {
        var request = new ProductSupplierRequest()
        {
            LotNumber = lot,
            Price = 12m,
            Stock = 7
        };

        return request;
    }
}
