using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Services;

public class ProductSupplierService : IProductSupplierService
{
    private readonly IProductSupplierRepository _productSuppliers;
    private readonly IProductRepository _products;
    private readonly ISupplierRepository _suppliers;

    public ProductSupplierService(
        IProductSupplierRepository productSuppliers,
        IProductRepository products,
        ISupplierRepository suppliers)
    {
        _productSuppliers = productSuppliers;
        _products = products;
        _suppliers = suppliers;
    }

    public async Task<IReadOnlyList<ProductSupplierResponse>?> GetProductSuppliersAsync(int productId)
    {
        if (await _products.GetByIdAsync(productId) == null)
        {
            return null;
        }

        var productSuppliers = await _productSuppliers.GetByProductAsync(productId);

        return productSuppliers.Select(ps => ToProductSupplierResponse(ps)).ToList();
    }

    public async Task<ProductSupplierResponse?> GetProductSupplierAsync(int productId, int id)
    {
        var productSupplier = await _productSuppliers.GetByIdAsync(productId, id);
        if (productSupplier == null)
        {
            return null;
        }

        return ToProductSupplierResponse(productSupplier);
    }

    public async Task<SaveProductSupplierResult> CreateProductSupplierAsync(
        int productId,
        CreateProductSupplierRequest request)
    {
        var lotNumber = NormalizeLot(request.LotNumber);
        var failure = await ValidateCreateAsync(productId, request.SupplierId, lotNumber);
        if (failure != null)
        {
            return ToSaveResult(failure.Value);
        }

        var productSupplier = ToProductSupplier(productId, lotNumber, request);
        await _productSuppliers.AddAsync(productSupplier);

        var created = await _productSuppliers.GetByIdAsync(productId, productSupplier.Id);
        return ToSaveResult(SaveProductSupplierStatus.Saved, ToProductSupplierResponse(created!));
    }

    public async Task<SaveProductSupplierResult> UpdateProductSupplierAsync(
        int productId,
        int id,
        ProductSupplierRequest request)
    {
        var productSupplier = await _productSuppliers.GetByIdForUpdateAsync(productId, id);
        if (productSupplier == null)
        {
            return ToSaveResult(SaveProductSupplierStatus.NotFound);
        }

        var lotNumber = NormalizeLot(request.LotNumber);
        if (await _productSuppliers.LotExistsAsync(productId, productSupplier.SupplierId, lotNumber, id))
        {
            return ToSaveResult(SaveProductSupplierStatus.DuplicateLot);
        }

        ApplyRequest(productSupplier, lotNumber, request);
        await _productSuppliers.SaveChangesAsync();
        return ToSaveResult(SaveProductSupplierStatus.Saved, ToProductSupplierResponse(productSupplier));
    }

    public async Task<DeleteProductSupplierResult> RemoveProductSupplierAsync(int productId, int id)
    {
        var productSupplier = await _productSuppliers.GetByIdForUpdateAsync(productId, id);
        if (productSupplier == null)
        {
            return DeleteProductSupplierResult.NotFound;
        }

        await _productSuppliers.RemoveAsync(productSupplier);
        return DeleteProductSupplierResult.Deleted;
    }

    private async Task<SaveProductSupplierStatus?> ValidateCreateAsync(
        int productId,
        int supplierId,
        string? lotNumber)
    {
        if (await _products.GetByIdAsync(productId) == null)
        {
            return SaveProductSupplierStatus.ProductNotFound;
        }

        if (!await _suppliers.ExistsAsync(supplierId))
        {
            return SaveProductSupplierStatus.SupplierNotFound;
        }

        if (await _productSuppliers.LotExistsAsync(productId, supplierId, lotNumber))
        {
            return SaveProductSupplierStatus.DuplicateLot;
        }

        return null;
    }

    private ProductSupplierResponse ToProductSupplierResponse(ProductSupplier productSupplier)
    {
        var response = new ProductSupplierResponse()
        {
            Id = productSupplier.Id,
            ProductId = productSupplier.ProductId,
            SupplierId = productSupplier.SupplierId,
            SupplierName = productSupplier.Supplier.Name,
            LotNumber = productSupplier.LotNumber,
            Price = productSupplier.Price,
            Stock = productSupplier.Stock
        };

        return response;
    }

    private ProductSupplier ToProductSupplier(
        int productId,
        string? lotNumber,
        CreateProductSupplierRequest request)
    {
        var productSupplier = new ProductSupplier()
        {
            ProductId = productId,
            SupplierId = request.SupplierId,
            LotNumber = lotNumber,
            Price = request.Price,
            Stock = request.Stock
        };

        return productSupplier;
    }

    private static SaveProductSupplierResult ToSaveResult(
        SaveProductSupplierStatus status,
        ProductSupplierResponse? productSupplier = null)
    {
        var result = new SaveProductSupplierResult()
        {
            Status = status,
            ProductSupplier = productSupplier
        };

        return result;
    }

    private void ApplyRequest(ProductSupplier productSupplier, string? lotNumber, ProductSupplierRequest request)
    {
        productSupplier.LotNumber = lotNumber;
        productSupplier.Price = request.Price;
        productSupplier.Stock = request.Stock;
    }

    private static string? NormalizeLot(string? lotNumber)
    {
        if (string.IsNullOrWhiteSpace(lotNumber))
        {
            return null;
        }

        return lotNumber.Trim();
    }
}
