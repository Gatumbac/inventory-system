# Full-stack walkthrough, backend half: add a brand to products

A complete example of the most likely kind of live change: one new field, end to end, searchable. Products have the most moving parts (attributes, code normalization), so if this works, the other recipes are smaller.

Request: *"Products need an optional brand (max 100 characters), shown in the list and searchable."*

Time target: about 15 minutes.

## 0. Before typing

- Say the plan: entity → migration → DTOs → service → search → test → Swagger → frontend model → form → list.
- Start the stack ([getting-started.md](getting-started.md)), and create a branch:

```bash
git switch -c feat/product-brand
```

## 1. Backend (from `backend/`)

### Entity: `src/Inventory.Core/Entities/Product.cs`

```csharp
public string? Brand { get; set; }
```

### Configuration: `src/Inventory.Infrastructure/Persistence/Configurations/ProductConfiguration.cs`

```csharp
builder.Property(p => p.Brand).HasMaxLength(100);
```

### Migration

Stop the API first.

```bash
dotenv -e ../.env -- dotnet ef migrations add AddProductBrand --project src/Inventory.Infrastructure --startup-project src/Inventory.Api --output-dir Persistence/Migrations
dotenv -e ../.env -- dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.Api
```

Open the generated `…_AddProductBrand.cs`. It should only contain `AddColumn<string>(name: "Brand", … maxLength: 100, nullable: true)`.

### DTOs

`src/Inventory.Core/Dtos/ProductRequest.cs`:

```csharp
[StringLength(100)]
public string? Brand { get; set; }
```

`src/Inventory.Core/Dtos/ProductResponse.cs`:

```csharp
public string? Brand { get; set; }
```

### Service: `src/Inventory.Core/Services/ProductService.cs`

- `ToProductResponse`: `Brand = product.Brand,`
- `ToProduct`: `Brand = productRequest.Brand?.Trim(),`
- `ApplyRequest`: `product.Brand = productRequest.Brand?.Trim();`

### Search: `src/Inventory.Infrastructure/Persistence/ProductRepository.cs`

```csharp
return query.Where(p => p.Code.Contains(term) || p.Name.Contains(term) || p.Brand!.Contains(term));
```

`Brand!` only silences the nullable warning. SQL Server's `LIKE` on a null column just does not match.

### Test: `tests/Inventory.Core.Tests/ProductServiceTests.cs`

Copy an existing create test and assert the trim:

```csharp
[Fact]
public async Task CreateProduct_TrimsBrand()
{
    var request = new ProductRequest()
    {
        Code = "AB-1",
        Name = "Laptop",
        Brand = "  Lenovo "
    };

    var result = await _service.CreateProductAsync(request);

    Assert.Equal("Lenovo", result.Product!.Brand);
}
```

Check the field names (`_service`) against the top of `ProductServiceTests.cs`.

```bash
dotnet format
dotnet test
dotenv -e ../.env -- dotnet run --project src/Inventory.Api
```

### Swagger check

http://localhost:5078/swagger → `POST /api/auth/login` (Admin) → copy `token` → **Authorize** → `PUT /api/products/{id}` with `"brand": "Lenovo"` → `GET /api/products?search=leno` returns it.

Next: the [frontend half](../../../frontend/docs/playbook/full-stack-walkthrough.md#2-frontend-from-frontendsrcappfeaturesproducts).

## 4. Commit

```bash
git add -A
git commit -m "feat(products): add optional brand to products"
```

## Bigger variants

| Request | Recipes |
| --- | --- |
| Brand as its own table, picked from a list | [backend/new-resource.md](new-resource.md) + [backend/relationships.md](relationships.md) + [frontend/dialogs-and-selects.md](../../../frontend/docs/playbook/dialogs-and-selects.md#c-pick-a-related-record-which-control) |
| Filter the list by brand | [backend/lists-filters-sorting.md](lists-filters-sorting.md) + [frontend/lists-filters.md](../../../frontend/docs/playbook/lists-filters.md) |
| Brand required | `[Required]` + `required string`, then a default for existing rows ([backend/migrations.md](migrations.md#adding-a-required-column-to-a-table-with-data)) |
