# Reports and read-only queries

**When:** "Show the total stock per product", "list products with low stock", "cheapest supplier for each product", "inventory value", "counts for a dashboard".

## Rules

- Do the work in SQL: `Select` into a DTO, and `Sum`/`Min`/`Count`/`GroupBy` inside the query. Never `ToListAsync()` everything and compute in memory.
- `AsNoTracking()` for every read.
- The soft-delete filters apply automatically, also inside navigation collections.
- `Sum` over an empty set returns 0 (EF adds `COALESCE`). `Min`/`Max`/`Average` over an empty set are SQL `NULL`: cast to `(decimal?)` or EF throws.

## Existing examples to copy

| Report | Backend | Frontend |
| --- | --- | --- |
| Dashboard: counts, total stock, inventory value, low-stock lots | `DashboardRepository` (`CountAsync`, `SumAsync`, `Where(ps => ps.Stock <= threshold)…Take(limit)`), `DashboardService`, `DashboardController` (`GET api/dashboard`) | `features/dashboard/` |
| Price matrix: products × suppliers, best price per cell | `ProductRepository.GetPagedWithSuppliersAsync` (`Include` + `ThenInclude`), `ProductService.GetPriceMatrixAsync` (pivot in memory over **one page**), `GET api/products/price-matrix` | `features/products/price-matrix/` |

For a new report, the closest of these two is usually the fastest start.

## Where the code goes

| Piece | File |
| --- | --- |
| Response DTO | `Core/Dtos/ProductStockResponse.cs` |
| Repository method | `IProductRepository` + `ProductRepository` (+ fake) |
| Service method | `IProductService` + `ProductService` |
| Endpoint | a new action in `ProductController`, or a new `ReportController` for several reports |

Since a report only reads and does not map entities, the repository may return the DTO directly from the projection. It is fine for `Core` DTOs to be built in `Infrastructure`, because `Infrastructure` already references `Core`.

## Example: stock and best price per product

DTO:

```csharp
namespace Inventory.Core.Dtos;

public class ProductStockResponse
{
    public required int ProductId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required int TotalStock { get; set; }
    public decimal? MinPrice { get; set; }
    public required int SupplierCount { get; set; }
}
```

Repository:

```csharp
public async Task<PagedResult<ProductStockResponse>> GetStockAsync(ProductFilterRequest filterRequest)
{
    var query = ApplySearch(_context.Products.AsNoTracking(), filterRequest.Search)
        .OrderBy(p => p.Name)
        .ThenBy(p => p.Id)
        .Select(p => new ProductStockResponse()
        {
            ProductId = p.Id,
            Code = p.Code,
            Name = p.Name,
            TotalStock = p.ProductSuppliers.Sum(ps => ps.Stock),
            MinPrice = p.ProductSuppliers.Min(ps => (decimal?)ps.Price),
            SupplierCount = p.ProductSuppliers.Select(ps => ps.SupplierId).Distinct().Count()
        });

    return await PagedQuery.ToPagedAsync(query, filterRequest);
}
```

`PagedQuery.ToPagedAsync` works on any `IQueryable<T>`, so the projection keeps the pagination.

Controller:

```csharp
[HttpGet("stock")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<PagedResult<ProductStockResponse>>> GetStock([FromQuery] ProductFilterRequest filterRequest)
{
    var response = await _productService.GetStockAsync(filterRequest);
    return Ok(response);
}
```

In ASP.NET Core routing a literal segment beats a parameter, so `GET /api/products/stock` reaches this action and not `GetProduct(int id)`.

## More query shapes

Low stock (threshold from the query string):

```csharp
.Where(p => p.ProductSuppliers.Sum(ps => ps.Stock) < threshold)
```

Products without suppliers:

```csharp
.Where(p => !p.ProductSuppliers.Any())
```

Cheapest supplier per product (name of the row with the minimum price):

```csharp
CheapestSupplier = p.ProductSuppliers
    .OrderBy(ps => ps.Price)
    .Select(ps => ps.Supplier.Name)
    .FirstOrDefault()
```

Totals per supplier (group by):

```csharp
var rows = await _context.ProductSuppliers
    .AsNoTracking()
    .GroupBy(ps => new { ps.SupplierId, ps.Supplier.Name })
    .Select(g => new SupplierTotalsResponse()
    {
        SupplierId = g.Key.SupplierId,
        SupplierName = g.Key.Name,
        Lots = g.Count(),
        TotalStock = g.Sum(ps => ps.Stock),
        InventoryValue = g.Sum(ps => ps.Price * ps.Stock)
    })
    .OrderBy(r => r.SupplierName)
    .ToListAsync();
```

Single numbers for a dashboard (this is `DashboardRepository`):

```csharp
return await _context.Products.CountAsync();
return await _context.ProductSuppliers.SumAsync(ps => ps.Price * ps.Stock);
```

To add a number to the dashboard: a method in `IDashboardRepository` + `DashboardRepository` + `FakeDashboardRepository`, a property in `DashboardResponse`, set it in `DashboardService.GetDashboardAsync`, then a card in `features/dashboard/dashboard.html` and the field in `dashboard.models.ts`.

Price matrix: see `ProductService.GetPriceMatrixAsync`. It loads one page of products with their lots and pivots that page in memory, which is fine because the page is at most 50 rows. Don't try to pivot in SQL with EF.

## Check the SQL

To see the generated SQL in the API console, change the `Microsoft.EntityFrameworkCore.Database.Command` override in `appsettings.json` from `Warning` to `Information` while you test. Change it back afterwards.

## Tests

The fake repositories hold lists in memory. For a report computed in the repository, the service just passes it through, so a unit test adds little. Show it in Swagger instead, and say that an integration test against a real database would be the right test for the query.

Next: [frontend/report-page.md](../frontend/report-page.md).
