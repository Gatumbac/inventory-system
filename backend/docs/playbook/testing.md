# Backend tests

xUnit, in `backend/tests/Inventory.Core.Tests`. Tests cover the **services** (business rules) with **fake repositories** that hold lists in memory. No database, no HTTP.

```
tests/Inventory.Core.Tests/
  Fakes/FakeSupplierRepository.cs   implements ISupplierRepository with a List<Supplier>
  SupplierServiceTests.cs           one class per service, one [Fact] per rule
  PagedResultTests.cs               a [Theory] example
```

## Commands

From `backend/`:

```bash
dotnet test
dotnet test --filter SupplierServiceTests
dotnet test --filter "FullyQualifiedName~CreateSupplier_DuplicateName"
dotnet test --logger "console;verbosity=normal"
```

Tests do not need `.env` or the database.

## Pattern

```csharp
public class SupplierServiceTests
{
    private readonly FakeSupplierRepository _suppliers = new FakeSupplierRepository();
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        _service = new SupplierService(_suppliers);
    }

    [Fact]
    public async Task CreateSupplier_DuplicateName_ReturnsDuplicateName()
    {
        _suppliers.Seed("Acme");

        var result = await _service.CreateSupplierAsync(NewRequest(" Acme"));

        Assert.Equal(SaveSupplierStatus.DuplicateName, result.Status);
        Assert.Single(_suppliers.Suppliers);
    }
}
```

- Name: `Method_Condition_ExpectedResult`.
- Arrange (seed the fake), Act (call the service), Assert. One blank line between the three.
- xUnit creates a new instance per test, so the fake is fresh every time.

## When you change a repository interface

The fake must implement the new method or the test project does not compile. Mirror the EF logic with LINQ over the list:

```csharp
public Task<bool> NameExistsAsync(string name, int? excludeId = null)
{
    var exists = Products.Any(p => p.Name == name && p.Id != excludeId);
    return Task.FromResult(exists);
}
```

For a flag-like check, use a set the test can fill, like `SupplierIdsWithProductSuppliers` in `FakeSupplierRepository`:

```csharp
public HashSet<int> CategoryIdsWithProducts { get; } = new HashSet<int>();

public Task<bool> HasProductsAsync(int categoryId)
{
    return Task.FromResult(CategoryIdsWithProducts.Contains(categoryId));
}
```

## When a service gets a new dependency

Add the fake in the test class and pass it to the constructor, like `ProductSupplierServiceTests` does with three fakes:

```csharp
_service = new ProductSupplierService(_productSuppliers, _products, _suppliers);
```

## Theory (several inputs, one rule)

```csharp
[Theory]
[InlineData(0, 10, 0)]
[InlineData(10, 10, 1)]
[InlineData(11, 10, 2)]
public void TotalPages_RoundsUp(int totalCount, int pageSize, int expected)
{
    var result = new PagedResult<int>()
    {
        Items = new List<int>(),
        Page = 1,
        PageSize = pageSize,
        TotalCount = totalCount
    };

    Assert.Equal(expected, result.TotalPages);
}
```

This is `PagedResultTests.cs`, unchanged.

## What to test in a live change

- Each new status the service can return (NotFound, Duplicate, rule broken).
- Trimming / normalization of new text fields.
- The happy path saves and maps the new field.

Endpoints (roles, DataAnnotations, routing) are not unit-tested in this repo. Check them in Swagger, and say that an integration test with `WebApplicationFactory` would be the next step.

## Before finishing

```bash
dotnet format
dotnet build
dotnet test
```
