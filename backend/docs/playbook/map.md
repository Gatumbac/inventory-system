# Backend map

.NET 10, EF Core with SQL Server, three projects. The dependencies point inward: `Api` → `Core` ← `Infrastructure`. `Core` knows nothing about EF or HTTP.

```
backend/
  src/Inventory.Core/            entities, DTOs, results, interfaces, services (business rules)
  src/Inventory.Infrastructure/  AppDbContext, EF configurations, repositories, migrations, seeder, JWT
  src/Inventory.Api/             controllers, Program.cs (DI, auth, CORS, Swagger), exception handler
  tests/Inventory.Core.Tests/    xUnit tests for services, with fake repositories
```

## Where each piece lives (Supplier as the template)

| # | Piece | File | What it does |
| --- | --- | --- | --- |
| 1 | Entity | `Core/Entities/Supplier.cs` | Table shape. Always `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`. |
| 2 | Request DTO | `Core/Dtos/SupplierRequest.cs` | Body for POST/PUT, with DataAnnotations (automatic 400). |
| 3 | Response DTO | `Core/Dtos/SupplierResponse.cs` | What the API returns. Never return entities. |
| 4 | Filter DTO | `Core/Dtos/SupplierFilterRequest.cs` | Query string for the list, extends `PageRequest` (`Page`, `PageSize`). |
| 5 | Results | `Core/Results/SaveSupplierResult.cs`, `DeleteSupplierResult.cs` | Status enums for expected outcomes (NotFound, Duplicate…). |
| 6 | Interfaces | `Core/Abstractions/ISupplierRepository.cs`, `ISupplierService.cs` | Contracts. Core depends on these, not on EF. |
| 7 | Service | `Core/Services/SupplierService.cs` | Business rules, trimming, mapping entity ↔ DTO. |
| 8 | EF configuration | `Infrastructure/Persistence/Configurations/SupplierConfiguration.cs` | Lengths, indexes, soft-delete filter, FKs. Picked up automatically by `ApplyConfigurationsFromAssembly`. |
| 9 | DbSet | `Infrastructure/Persistence/AppDbContext.cs` | `public DbSet<Supplier> Suppliers { get; set; }` |
| 10 | Repository | `Infrastructure/Persistence/SupplierRepository.cs` | EF queries. Lists use `PagedQuery.ToPagedAsync`. |
| 11 | DI | `Api/Program.cs` | `AddScoped<ISupplierRepository, SupplierRepository>()` and the service. |
| 12 | Controller | `Api/Controllers/SupplierController.cs` | Route, roles, status codes, maps results to `Problem(...)`. |
| 13 | Messages | `Core/Constants/ErrorMessages.cs` | Spanish messages for the user. |
| 14 | Test fake | `tests/Inventory.Core.Tests/Fakes/FakeSupplierRepository.cs` | In-memory repository. |
| 15 | Tests | `tests/Inventory.Core.Tests/SupplierServiceTests.cs` | One `[Fact]` per rule. |
| 16 | Migration | `Infrastructure/Persistence/Migrations/` | Generated, see [migrations.md](migrations.md). |

Products follow the same set, plus `ProductAttribute` (child rows replaced on update) and `ProductSupplier` (lots: price and stock per supplier, nested route `api/products/{productId}/suppliers`).

Other resources and what they are good templates for:

| Resource | Endpoints | Copy it for |
| --- | --- | --- |
| Users | `api/users` (admin only): list with `inactive` filter, create, deactivate, `POST {id}/reactivate` | Admin-only controller, current-user rules, restore of soft-deleted rows |
| Dashboard | `GET api/dashboard` | Read-only aggregates (counts, sums, low stock) with its own repository |
| Price matrix | `GET api/products/price-matrix` | Report built from a page of products with `Include`/`ThenInclude` |

## Request flow

```
HTTP → Controller ([Authorize], DataAnnotations → 400)
     → Service (rules → typed result)
     → Repository (EF, AsNoTracking for reads)
     → AppDbContext (+ AuditSaveChangesInterceptor stamps dates)
← Controller maps the result: Ok / CreatedAtAction / NoContent / Problem(404|409, title: ErrorMessages.X)
```

Unhandled exceptions go to `Api/Middleware/GlobalExceptionHandler.cs`: logged, and returned as a 500 with `ErrorMessages.UnexpectedError`.

## Conventions to keep

- Expected failures are **typed results**, not exceptions (`SaveSupplierStatus.DuplicateName` → 409).
- Messages for the user are in Spanish, in `ErrorMessages`. Everything else is in English.
- Every error carries its message in `title`, and the frontend shows `title`.
- **Soft delete only**: set `DeletedAt`. A global query filter hides deleted rows.
- Trim text before comparing or saving.
- Lists: `page` + `pageSize` (max 50), ordered by a column and then by `Id` for stable pages.
- Objects created with `new X()` and one property per line. No comments.
- Reads use `AsNoTracking()`. Updates load with `GetByIdForUpdateAsync` (tracked) and call `SaveChangesAsync`.

## Recipes

- [add-field.md](add-field.md)
- [new-resource.md](new-resource.md)
- [relationships.md](relationships.md)
- [lists-filters-sorting.md](lists-filters-sorting.md)
- [validation-and-rules.md](validation-and-rules.md)
- [reports-and-queries.md](reports-and-queries.md)
- [auth-and-roles.md](auth-and-roles.md)
- [data-lifecycle.md](data-lifecycle.md)
- [logging-and-errors.md](logging-and-errors.md)
- [testing.md](testing.md)
- [migrations.md](migrations.md)
