using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _suppliers;

    public SupplierService(ISupplierRepository suppliers)
    {
        _suppliers = suppliers;
    }

    public async Task<PagedResult<SupplierResponse>> GetSuppliersAsync(SupplierFilterRequest filterRequest)
    {
        var page = await _suppliers.GetPagedAsync(filterRequest);

        return page.Map(s => ToSupplierResponse(s));
    }

    public async Task<SupplierResponse?> GetSupplierAsync(int id)
    {
        var supplier = await _suppliers.GetByIdAsync(id);
        if (supplier == null)
        {
            return null;
        }

        return ToSupplierResponse(supplier);
    }

    public async Task<SaveSupplierResult> CreateSupplierAsync(SupplierRequest supplierRequest)
    {
        var supplier = ToSupplier(supplierRequest);
        if (await _suppliers.NameExistsAsync(supplier.Name))
        {
            return ToSaveResult(SaveSupplierStatus.DuplicateName);
        }

        await _suppliers.AddAsync(supplier);
        return ToSaveResult(SaveSupplierStatus.Saved, ToSupplierResponse(supplier));
    }

    public async Task<SaveSupplierResult> UpdateSupplierAsync(int id, SupplierRequest supplierRequest)
    {
        var supplier = await _suppliers.GetByIdForUpdateAsync(id);
        if (supplier == null)
        {
            return ToSaveResult(SaveSupplierStatus.NotFound);
        }

        var name = NormalizeName(supplierRequest.Name);
        if (await _suppliers.NameExistsAsync(name, id))
        {
            return ToSaveResult(SaveSupplierStatus.DuplicateName);
        }

        ApplyRequest(supplier, name, supplierRequest);
        await _suppliers.SaveChangesAsync();
        return ToSaveResult(SaveSupplierStatus.Saved, ToSupplierResponse(supplier));
    }

    public async Task<DeleteSupplierResult> RemoveSupplierAsync(int id)
    {
        var supplier = await _suppliers.GetByIdForUpdateAsync(id);
        if (supplier == null)
        {
            return DeleteSupplierResult.NotFound;
        }

        if (await _suppliers.HasProductSuppliersAsync(id))
        {
            return DeleteSupplierResult.HasProductSuppliers;
        }

        await _suppliers.RemoveAsync(supplier);
        return DeleteSupplierResult.Deleted;
    }

    private SupplierResponse ToSupplierResponse(Supplier supplier)
    {
        var response = new SupplierResponse()
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Description = supplier.Description
        };

        return response;
    }

    private Supplier ToSupplier(SupplierRequest supplierRequest)
    {
        var supplier = new Supplier()
        {
            Name = NormalizeName(supplierRequest.Name),
            Description = supplierRequest.Description?.Trim()
        };

        return supplier;
    }

    private static SaveSupplierResult ToSaveResult(SaveSupplierStatus status, SupplierResponse? supplier = null)
    {
        var result = new SaveSupplierResult()
        {
            Status = status,
            Supplier = supplier
        };

        return result;
    }

    private void ApplyRequest(Supplier supplier, string name, SupplierRequest supplierRequest)
    {
        supplier.Name = name;
        supplier.Description = supplierRequest.Description?.Trim();
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
    }
}
