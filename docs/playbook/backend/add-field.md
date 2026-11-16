# Add a field to an existing entity

**When:** "Suppliers need a phone", "products need a unit of measure / minimum stock / brand".

Example: an optional `Phone` on `Supplier`, max 20 characters. For `Product`, the files have the same names with `Product` instead of `Supplier`.

## Files, in order

### 1. Entity: `Core/Entities/Supplier.cs`

```csharp
public string? Phone { get; set; }
```

Use `string?` if it is optional, and `required string` if it is mandatory. For numbers: `int`, `decimal`, `bool`, `DateTime?`.

### 2. EF configuration: `Infrastructure/Persistence/Configurations/SupplierConfiguration.cs`

```csharp
builder.Property(s => s.Phone).HasMaxLength(20);
```

Other options you may need:

```csharp
builder.Property(p => p.MinStock).HasDefaultValue(0);
builder.Property(p => p.Cost).HasPrecision(18, 2);
builder.HasIndex(s => s.Phone);
builder.ToTable(t => t.HasCheckConstraint("CK_Products_MinStock_NonNegative", "[MinStock] >= 0"));
```

### 3. Request DTO: `Core/Dtos/SupplierRequest.cs`

```csharp
[StringLength(20)]
public string? Phone { get; set; }
```

Validation attributes return a 400 before the service runs. Examples:

```csharp
[Required]
[Range(0, int.MaxValue, ErrorMessage = ErrorMessages.MinStockInvalid)]
[RegularExpression("^[0-9+ -]+$", ErrorMessage = ErrorMessages.PhoneInvalid)]
[EmailAddress]
```

If you use a custom `ErrorMessage`, add the constant to `Core/Constants/ErrorMessages.cs` (in Spanish).

### 4. Response DTO: `Core/Dtos/SupplierResponse.cs`

```csharp
public string? Phone { get; set; }
```

### 5. Service mapping: `Core/Services/SupplierService.cs`

There are three places, and each one is easy to forget:

```csharp
private SupplierResponse ToSupplierResponse(Supplier supplier)
{
    var response = new SupplierResponse()
    {
        Id = supplier.Id,
        Name = supplier.Name,
        Description = supplier.Description,
        Phone = supplier.Phone
    };

    return response;
}

private Supplier ToSupplier(SupplierRequest supplierRequest)
{
    var supplier = new Supplier()
    {
        Name = NormalizeName(supplierRequest.Name),
        Description = supplierRequest.Description?.Trim(),
        Phone = supplierRequest.Phone?.Trim()
    };

    return supplier;
}

private void ApplyRequest(Supplier supplier, string name, SupplierRequest supplierRequest)
{
    supplier.Name = name;
    supplier.Description = supplierRequest.Description?.Trim();
    supplier.Phone = supplierRequest.Phone?.Trim();
}
```

`ToSupplier` is for create, `ApplyRequest` is for update, and `ToSupplierResponse` is for every read.

### 6. Migration

```bash
dotenv -e ../.env -- dotnet ef migrations add AddSupplierPhone --project src/Inventory.Infrastructure --startup-project src/Inventory.Api --output-dir Persistence/Migrations
dotenv -e ../.env -- dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.Api
```

See [migrations.md](migrations.md) for required columns on tables that already have rows.

### 7. Test: `tests/Inventory.Core.Tests/SupplierServiceTests.cs`

```csharp
[Fact]
public async Task CreateSupplier_TrimsPhone()
{
    var request = new SupplierRequest()
    {
        Name = "Acme",
        Phone = " 099 123 "
    };

    var result = await _service.CreateSupplierAsync(request);

    Assert.Equal("099 123", result.Supplier!.Phone);
}
```

### 8. Optional: seed data and search

- Seed: add the value in `DbSeeder` (see [data-lifecycle.md](data-lifecycle.md#seed-data)).
- Search by the new field: [lists-filters-sorting.md](lists-filters-sorting.md).

## Verify

1. `dotnet build` and `dotnet test`.
2. Swagger: `PUT /api/suppliers/{id}` with `phone`, then `GET` returns it.
3. A value that is too long returns 400 with the message in `errors`.

Next: [frontend/add-field.md](../frontend/add-field.md).
