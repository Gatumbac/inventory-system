using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Results;

namespace Inventory.Core.Tests.Fakes;

public class FakeProductRepository : IProductRepository
{
    private int _nextId = 1;

    public List<Product> Products { get; } = new List<Product>();
    public HashSet<int> ProductIdsWithSuppliers { get; } = new HashSet<int>();
    public int SaveChangesCount { get; private set; }
    public List<Product> Removed { get; } = new List<Product>();

    public Product Seed(string code)
    {
        var product = new Product()
        {
            Id = _nextId++,
            Code = code,
            Name = "Seeded"
        };

        Products.Add(product);
        return product;
    }

    public Task<PagedResult<Product>> GetPagedAsync(ProductFilterRequest filterRequest)
    {
        var page = new PagedResult<Product>()
        {
            Items = Products.ToList(),
            Page = filterRequest.Page,
            PageSize = filterRequest.PageSize,
            TotalCount = Products.Count
        };

        return Task.FromResult(page);
    }

    public Task<PagedResult<Product>> GetPagedWithSuppliersAsync(ProductFilterRequest filterRequest)
    {
        return GetPagedAsync(filterRequest);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return Task.FromResult(Products.FirstOrDefault(p => p.Id == id));
    }

    public Task<Product?> GetByIdForUpdateAsync(int id)
    {
        return GetByIdAsync(id);
    }

    public Task AddAsync(Product product)
    {
        product.Id = _nextId++;
        Products.Add(product);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        SaveChangesCount++;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Product product)
    {
        Products.Remove(product);
        Removed.Add(product);
        return Task.CompletedTask;
    }

    public Task<bool> HasSuppliersAsync(int productId)
    {
        return Task.FromResult(ProductIdsWithSuppliers.Contains(productId));
    }

    public Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var exists = Products.Any(p => p.Code == code && p.Id != excludeId);
        return Task.FromResult(exists);
    }
}
