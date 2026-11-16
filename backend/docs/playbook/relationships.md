# Relationships

The model today:

```
Role 1─* User
Product 1─* ProductAttribute          (owned children, replaced on update)
Product 1─* ProductSupplier *─1 Supplier   (many-to-many with payload: LotNumber, Price, Stock)
```

## A. One-to-many: a product belongs to a category

**When:** "Each product has a category", "each supplier belongs to a city".

### Entity

```csharp
public class Product
{
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}

public class Category
{
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
```

Start with `int?` (optional): existing products have no category, and a required FK on a table with rows would fail or need a fake default. Make it required later if the request insists, after seeding a category for every product.

### Configuration: `ProductConfiguration.cs`

```csharp
builder.HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);
```

Use `Restrict`, the same as `ProductSupplierConfiguration`. Deletes are soft anyway, so the repository must check for children (below).

### DTOs

```csharp
public class ProductRequest
{
    [Range(1, int.MaxValue)]
    public int? CategoryId { get; set; }
}

public class ProductResponse
{
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
```

Return the name too, so the list does not need a second call.

### Service: validate that the parent exists

Add a status to `SaveProductStatus`:

```csharp
public enum SaveProductStatus
{
    Saved,
    NotFound,
    DuplicateCode,
    CategoryNotFound
}
```

In `CreateProductAsync` and `UpdateProductAsync`, before saving:

```csharp
if (productRequest.CategoryId != null && !await _categories.ExistsAsync(productRequest.CategoryId.Value))
{
    return ToSaveResult(SaveProductStatus.CategoryNotFound);
}
```

`ProductService` now needs `ICategoryRepository` in its constructor, and the test has to pass a `FakeCategoryRepository`. `ExistsAsync` respects the soft-delete filter, so a deleted category counts as missing.

Map it: `CategoryId = productRequest.CategoryId` in `ToProduct` and `ApplyRequest`, and `CategoryId`/`CategoryName = product.Category?.Name` in `ToProductResponse`.

### Repository: load the parent

In `ProductRepository`, every read that maps to a response:

```csharp
var query = _context.Products
    .AsNoTracking()
    .Include(p => p.Attributes)
    .Include(p => p.Category);
```

Also in `GetByIdAsync` and `GetByIdForUpdateAsync`. Without the `Include`, `CategoryName` is always null.

After an update, the tracked `product.Category` still points to the old category. If the response needs the new name, reload it with `GetByIdAsync(id)` before mapping.

### Controller

```csharp
if (result.Status == SaveProductStatus.CategoryNotFound)
{
    return Problem(statusCode: StatusCodes.Status404NotFound, title: ErrorMessages.CategoryNotFound);
}
```

Some teams prefer 400 or 422 here, since the product route exists but the body points to a missing category. Pick one and say why.

### Delete guard on the parent

`CategoryRepository`:

```csharp
public async Task<bool> HasProductsAsync(int categoryId)
{
    return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
}
```

The service returns `DeleteCategoryResult.HasProducts`, and the controller maps it to 409 with `ErrorMessages.CategoryHasProducts`. It is the same as `SupplierService.RemoveSupplierAsync` with `HasProductSuppliersAsync`.

### Migration

`AddProductCategory`. See [migrations.md](migrations.md).

## B. Many-to-many with data: the ProductSupplier pattern

**When:** "A product can be in several warehouses with a quantity in each", "a supplier offers products with a price".

Copy the `ProductSupplier` set:

| File | Role |
| --- | --- |
| `Core/Entities/ProductSupplier.cs` | Join entity with its own `Id` and payload columns |
| `Infrastructure/.../ProductSupplierConfiguration.cs` | Two `HasOne(...).WithMany(...).OnDelete(Restrict)`, unique index on the combination with `HasFilter("[DeletedAt] IS NULL")`, check constraints |
| `Core/Dtos/CreateProductSupplierRequest.cs` | Create carries the other side's id (`SupplierId`), update does not |
| `Core/Services/ProductSupplierService.cs` | Checks that both sides exist, duplicate combination → status |
| `Api/Controllers/ProductSupplierController.cs` | Nested route `api/products/{productId}/suppliers`, `ToFailure` switch for statuses |

The join entity has its own `Id` (not a composite key) so it can be soft-deleted and edited by id.

## C. One-to-many owned children: the ProductAttribute pattern

**When:** "A product has several barcodes / images / tags", edited together with the product in one form.

- The child has `ProductId` and no own endpoint.
- The request carries a list (`List<ProductAttributeRequest>`).
- On update, the service clears and re-adds: `product.Attributes.Clear()` and then `Add` for each item (see `ProductService.ApplyRequest`).
- The repository loads them with `.Include(p => p.Attributes)` in every read.
- Rules about the whole list go in `IValidatableObject.Validate` on the request (see the unique-names rule in `ProductRequest`).

## Verify

- Swagger: create with a valid parent id → 201 and the name in the response. Non-existing id → 404/400. Delete a parent that still has children → 409.
- `dotnet test`.
