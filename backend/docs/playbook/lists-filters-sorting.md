# Lists: filters, search and sorting

Every list endpoint takes a `XFilterRequest : PageRequest` from the query string and returns `PagedResult<T>` (`items`, `page`, `pageSize`, `totalCount`, `totalPages`).

- `PageRequest` (`Core/Dtos/PageRequest.cs`): `Page` 1..100000, `PageSize` 1..50, defaults 1 and 10.
- `PagedQuery.ToPagedAsync(query, request)` (`Infrastructure/Persistence/PagedQuery.cs`): count + `Skip`/`Take`.
- `PagedResult.Map(...)` (`Core/Results/PagedResult.cs`): entity page → DTO page in the service.

A working example of a filter beyond search: `UserFilterRequest.Inactive` (bool) switches `UserRepository.GetPagedAsync` between active and soft-deleted users.

## A. New filter

**When:** "Filter products by supplier", "suppliers by name only", "products with stock below N".

### 1. DTO: `Core/Dtos/ProductFilterRequest.cs`

```csharp
public class ProductFilterRequest : PageRequest
{
    [StringLength(100)]
    public string? Search { get; set; }

    [Range(1, int.MaxValue)]
    public int? SupplierId { get; set; }
}
```

It binds from `?supplierId=3` automatically (`[FromQuery]` in the controller). Optional filters are nullable.

### 2. Repository: `ProductRepository.GetPagedAsync`

Add a private `ApplyX` per filter, like the existing `ApplySearch`:

```csharp
var products = ApplySupplier(ApplySearch(query, filterRequest.Search), filterRequest.SupplierId)
    .OrderBy(p => p.Name)
    .ThenBy(p => p.Id);
```

```csharp
private static IQueryable<Product> ApplySupplier(IQueryable<Product> query, int? supplierId)
{
    if (supplierId == null)
    {
        return query;
    }

    return query.Where(p => p.ProductSuppliers.Any(ps => ps.SupplierId == supplierId));
}
```

`ApplySearch` returns `IQueryable<Product>`, so the chain still works after `.Include`. The soft-delete filter also applies inside `Any(...)`, so deleted lots do not match.

More filter shapes:

```csharp
query.Where(p => p.CategoryId == categoryId);
query.Where(p => p.ProductSuppliers.Sum(ps => ps.Stock) < threshold);
query.Where(p => p.CreatedAt >= from && p.CreatedAt < to);
query.Where(p => p.Name.StartsWith(term));
```

### 3. Fake repository

`FakeProductRepository.GetPagedAsync` ignores filters. If you test the filter, make it real in the fake, or test the repository by hand in Swagger.

## B. Search in more columns

`ApplySearch` already does `Contains` (SQL `LIKE '%term%'`). To include another column:

```csharp
return query.Where(p => p.Code.Contains(term) || p.Name.Contains(term) || p.Description!.Contains(term));
```

SQL Server's default collation is case-insensitive, so there is no need for `ToLower()`.

## C. Sorting chosen by the client

**When:** "Let the user sort by name or code".

DTO:

```csharp
[RegularExpression("^(name|code|createdAt)$")]
public string? SortBy { get; set; }

public bool Descending { get; set; }
```

Repository:

```csharp
private static IQueryable<Product> ApplySort(IQueryable<Product> query, string? sortBy, bool descending)
{
    var sorted = sortBy switch
    {
        "code" => descending ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
        "createdAt" => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
        _ => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
    };

    return sorted.ThenBy(p => p.Id);
}
```

Always end with `ThenBy(Id)`. Without it, rows with the same name can jump between pages.

Use a whitelist (`switch` + `RegularExpression`), never a column name taken straight from the request.

## D. Bigger page size

`PageRequest.MaxPageSize = 50` is used by both the `[Range]` and `ErrorMessages.PageSizeInvalid`. If you change it, update the message text too.

## Verify

- Swagger: `GET /api/products?supplierId=1&page=1&pageSize=5` → only matching items, and `totalCount` matches.
- No filter → same result as before.
- Invalid value (`supplierId=0`) → 400.

Next: [frontend/lists-filters.md](../../../frontend/docs/playbook/lists-filters.md).
