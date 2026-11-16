# New CRUD resource

**When:** "Add categories / warehouses / brands that the admin can manage."

Copy the **Supplier** file set and rename. It is the simplest complete CRUD: list with search and pagination, get, create, update, soft delete, unique name, delete guard.

Example: `Category` (name, optional description, unique name).

## 1. Copy and rename

From `backend/`:

```bash
cp src/Inventory.Core/Entities/Supplier.cs                src/Inventory.Core/Entities/Category.cs
cp src/Inventory.Core/Dtos/SupplierRequest.cs             src/Inventory.Core/Dtos/CategoryRequest.cs
cp src/Inventory.Core/Dtos/SupplierResponse.cs            src/Inventory.Core/Dtos/CategoryResponse.cs
cp src/Inventory.Core/Dtos/SupplierFilterRequest.cs       src/Inventory.Core/Dtos/CategoryFilterRequest.cs
cp src/Inventory.Core/Results/SaveSupplierResult.cs       src/Inventory.Core/Results/SaveCategoryResult.cs
cp src/Inventory.Core/Results/DeleteSupplierResult.cs     src/Inventory.Core/Results/DeleteCategoryResult.cs
cp src/Inventory.Core/Abstractions/ISupplierRepository.cs src/Inventory.Core/Abstractions/ICategoryRepository.cs
cp src/Inventory.Core/Abstractions/ISupplierService.cs    src/Inventory.Core/Abstractions/ICategoryService.cs
cp src/Inventory.Core/Services/SupplierService.cs         src/Inventory.Core/Services/CategoryService.cs
cp src/Inventory.Infrastructure/Persistence/Configurations/SupplierConfiguration.cs src/Inventory.Infrastructure/Persistence/Configurations/CategoryConfiguration.cs
cp src/Inventory.Infrastructure/Persistence/SupplierRepository.cs src/Inventory.Infrastructure/Persistence/CategoryRepository.cs
cp src/Inventory.Api/Controllers/SupplierController.cs    src/Inventory.Api/Controllers/CategoryController.cs
cp tests/Inventory.Core.Tests/Fakes/FakeSupplierRepository.cs tests/Inventory.Core.Tests/Fakes/FakeCategoryRepository.cs
cp tests/Inventory.Core.Tests/SupplierServiceTests.cs     tests/Inventory.Core.Tests/CategoryServiceTests.cs
```

Then replace the names in the new files only, **matching case** (in the editor: find and replace inside the file, with "match case" on):

| Find | Replace |
| --- | --- |
| `Supplier` | `Category` |
| `supplier` | `category` |
| `Suppliers` | `Categories` (the DbSet, the route `api/categories`) |
| `suppliers` | `categories` |

After replacing, check these spots by hand:

- Entity: delete `ProductSuppliers`. Add `ICollection<Product> Products` only if products will reference it ([relationships.md](relationships.md)).
- Repository: `HasProductSuppliersAsync` becomes `HasProductsAsync` (or delete it and the `DeleteCategoryResult.HasProductSuppliers` value if nothing can reference a category).
- Route in the controller: `[Route("api/categories")]`.
- Lengths in the DTO, the configuration and the frontend validators must match.

## 2. Messages: `Core/Constants/ErrorMessages.cs`

```csharp
public const string CategoryNotFound = "La categoría no existe";
public const string CategoryNameDuplicated = "Ya existe una categoría con ese nombre";
public const string CategoryHasProducts = "No se puede eliminar una categoría con productos asociados";
```

## 3. DbSet: `Infrastructure/Persistence/AppDbContext.cs`

```csharp
public DbSet<Category> Categories { get; set; }
```

The configuration class is picked up automatically (`ApplyConfigurationsFromAssembly`).

## 4. DI: `Api/Program.cs`

Next to the other registrations:

```csharp
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
```

If you forget this, you get a 500 with `Unable to resolve service for type 'ICategoryService'`.

## 5. Migration

```bash
dotenv -e ../.env -- dotnet ef migrations add AddCategories --project src/Inventory.Infrastructure --startup-project src/Inventory.Api --output-dir Persistence/Migrations
dotenv -e ../.env -- dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.Api
```

## 6. Controller roles

The copy keeps the Supplier rules: `[Authorize]` on the class (any logged-in user can read) and `[Authorize(Roles = RoleNames.Admin)]` on POST, PUT and DELETE. Change them only if the request says so ([auth-and-roles.md](auth-and-roles.md)).

## 7. Tests

The copied tests compile after the rename. Run them:

```bash
dotnet test --filter CategoryServiceTests
```

## Smaller variants

| Request | What to drop |
| --- | --- |
| Read-only list (no create/edit) | Keep entity, Response, Filter, repository `GetPagedAsync`/`GetByIdAsync`, service reads, controller GETs. Drop Request, Results and writes. |
| No pagination (small lookup list) | Repository returns `Task<IReadOnlyList<Category>>` with `.OrderBy(c => c.Name).ToListAsync()`. The controller returns `ActionResult<IReadOnlyList<CategoryResponse>>`. |
| Child of a product (one product, many X) | Use the `ProductSupplier` set instead: nested route `api/products/{productId}/...` ([relationships.md](relationships.md)). |

## Verify

1. `dotnet build`, `dotnet test`.
2. Swagger, as Admin: POST → 201, POST the same name again → 409, GET list → paged, PUT → 200, DELETE → 204, GET by id → 404.
3. As Viewer: POST → 403.

Next: [frontend/new-screen.md](../../../frontend/docs/playbook/new-screen.md).
