using Inventory.Core.Dtos;
using Inventory.Core.Results;
using Inventory.Core.Services;
using Inventory.Core.Tests.Fakes;

namespace Inventory.Core.Tests;

public class SupplierServiceTests
{
    private readonly FakeSupplierRepository _suppliers = new FakeSupplierRepository();
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        _service = new SupplierService(_suppliers);
    }

    [Fact]
    public async Task CreateSupplier_TrimsNameAndSaves()
    {
        var result = await _service.CreateSupplierAsync(NewRequest("  Acme "));

        Assert.Equal(SaveSupplierStatus.Saved, result.Status);
        Assert.Equal("Acme", result.Supplier!.Name);
    }

    [Fact]
    public async Task CreateSupplier_DuplicateName_ReturnsDuplicateName()
    {
        _suppliers.Seed("Acme");

        var result = await _service.CreateSupplierAsync(NewRequest(" Acme"));

        Assert.Equal(SaveSupplierStatus.DuplicateName, result.Status);
        Assert.Single(_suppliers.Suppliers);
    }

    [Fact]
    public async Task RemoveSupplier_Missing_ReturnsNotFound()
    {
        var result = await _service.RemoveSupplierAsync(99);

        Assert.Equal(DeleteSupplierResult.NotFound, result);
    }

    [Fact]
    public async Task RemoveSupplier_WithProductSuppliers_ReturnsHasProductSuppliersAndKeepsSupplier()
    {
        var supplier = _suppliers.Seed("Acme");
        _suppliers.SupplierIdsWithProductSuppliers.Add(supplier.Id);

        var result = await _service.RemoveSupplierAsync(supplier.Id);

        Assert.Equal(DeleteSupplierResult.HasProductSuppliers, result);
        Assert.Empty(_suppliers.Removed);
    }

    [Fact]
    public async Task RemoveSupplier_WithoutProductSuppliers_Deletes()
    {
        var supplier = _suppliers.Seed("Acme");

        var result = await _service.RemoveSupplierAsync(supplier.Id);

        Assert.Equal(DeleteSupplierResult.Deleted, result);
        Assert.Single(_suppliers.Removed);
    }

    private static SupplierRequest NewRequest(string name)
    {
        var request = new SupplierRequest()
        {
            Name = name
        };

        return request;
    }
}
