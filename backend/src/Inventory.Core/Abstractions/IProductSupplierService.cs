using Inventory.Core.Dtos;
using Inventory.Core.Results;

namespace Inventory.Core.Abstractions;

public interface IProductSupplierService
{
    Task<IReadOnlyList<ProductSupplierResponse>?> GetProductSuppliersAsync(int productId);
    Task<ProductSupplierResponse?> GetProductSupplierAsync(int productId, int id);
    Task<SaveProductSupplierResult> CreateProductSupplierAsync(int productId, CreateProductSupplierRequest request);
    Task<SaveProductSupplierResult> UpdateProductSupplierAsync(int productId, int id, ProductSupplierRequest request);
    Task<DeleteProductSupplierResult> RemoveProductSupplierAsync(int productId, int id);
}
