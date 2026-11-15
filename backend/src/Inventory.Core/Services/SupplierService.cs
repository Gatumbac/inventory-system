using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;

namespace Inventory.Core.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _suppliers;

    public SupplierService(ISupplierRepository suppliers)
    {
        _suppliers = suppliers;
    }

    public async Task<IReadOnlyList<SupplierResponse>> GetSuppliersAsync()
    {
        var suppliers = await _suppliers.GetAllAsync();

        return suppliers.Select(s => ToSupplierResponse(s)).ToList();
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
}
