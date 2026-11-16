# Data lifecycle: soft delete, audit dates, seed data

## Soft delete

Nothing is physically deleted. `RemoveAsync` in every repository does:

```csharp
supplier.DeletedAt = DateTime.UtcNow;
await _context.SaveChangesAsync();
```

And every configuration hides deleted rows:

```csharp
builder.HasQueryFilter(s => s.DeletedAt == null);
```

Consequences:

- Every query, including `Include`, `Any` and `Count` on navigations, skips deleted rows automatically.
- Unique indexes use `.HasFilter("[DeletedAt] IS NULL")`, so a deleted name can be reused.
- The database FK does not stop you from soft-deleting a parent with children. The repository checks first (`HasProductSuppliersAsync`, `HasSuppliersAsync`) and the service returns a `Has…` status → 409.

New entities: add `DateTime? DeletedAt`, the query filter, the filter on unique indexes, and set `DeletedAt` in `RemoveAsync`.

### Restore a deleted row

**When:** "Admins can restore deleted suppliers", "show deleted products".

Users already do this. Copy it:

| Piece | Where |
| --- | --- |
| List inactive rows | `UserRepository` (`IgnoreQueryFilters().Where(u => u.DeletedAt != null)` when the filter asks for inactive) |
| Load a deleted row | `UserRepository` (`IgnoreQueryFilters()` + `DeletedAt != null`) |
| Restore | `UserService.ReactivateUserAsync` → `ReactivateUserResult` (`NotFound`, `DuplicateEmail`, `Reactivated`) |
| Endpoint | `UserController`: `POST api/users/{id}/reactivate` |
| UI | `features/users/user-list` (inactive view + reactivate action) |

The same for suppliers:

Repository:

```csharp
public async Task<Supplier?> GetDeletedForUpdateAsync(int id)
{
    return await _context.Suppliers
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt != null);
}
```

Service: if the row is not found → `NotFound`. If an active row already uses the name (`NameExistsAsync(supplier.Name)`) → `DuplicateName`, since restoring would break the unique index. Otherwise set `supplier.DeletedAt = null` and `SaveChangesAsync`.

Controller: `[HttpPost("{id}/restore")]` with `[Authorize(Roles = RoleNames.Admin)]`.

List deleted rows: same as `GetPagedAsync` with `.IgnoreQueryFilters().Where(s => s.DeletedAt != null)`. `IgnoreQueryFilters` turns off **all** filters on that query, including the ones on included navigations.

### Real delete

If the request explicitly asks for a physical delete: `_context.Suppliers.Remove(supplier)`. The FKs are `Restrict`, so the delete fails with a DB error if children exist. Keep the `Has…` check before it.

## Audit dates

`AuditSaveChangesInterceptor` (registered in `Program.cs` with `AddInterceptors`) fills `CreatedAt` and `UpdatedAt` for any entity that has both properties:

- Added → both set to `DateTime.UtcNow`.
- Modified → `UpdatedAt`.

New entities only need the two properties. Do not set them by hand.

**When:** "Show the creation date in the list": add `CreatedAt` to the Response DTO and map it. The frontend formats it with `DatePipe` ([frontend/report-page.md](../../../frontend/docs/playbook/report-page.md#pipes)).

## Seed data

`Infrastructure/Persistence/DbSeeder.cs`, wired in `Program.cs`:

```csharp
.UseSeeding((context, _) => DbSeeder.Seed(context, builder.Configuration, new IdentityPasswordHasher()))
```

It runs on `dotnet ef database update` and in the Docker `migrate` bundle, not when the API starts. Every helper checks first, so running it again only adds what is missing.

Add rows with the existing helpers:

```csharp
EnsureSupplier(context, "Nuevo Proveedor", "Descripción");
EnsureProduct(context, "HUB-001", "Hub USB-C", "Hub de 7 puertos con HDMI");
EnsureProductSupplier(context, "HUB-001", "Nuevo Proveedor", "L-2026-01", 45.00m, 10);
EnsureProductAttribute(context, "HUB-001", "Puertos", "7");
```

Check the codes already used in `DbSeeder` first (about 26 products and 10 suppliers). An `Ensure*` with an existing code or name does nothing.

Keep the order in `Seed`: suppliers and products before the rows that point to them (`EnsureProductSupplier` uses `Single(...)` and throws if the parent is missing).

For a new entity, write `EnsureCategory` by copying `EnsureSupplier`, and call it from `Seed` before anything that references categories.

If you add a new field and want existing seeded rows to have it, note that the `Ensure*` helpers skip rows that already exist. Either reset the database (`docker compose down -v`, `up -d db`, `database update`) or update the rows in a migration with `migrationBuilder.Sql("UPDATE ...")`.

Apply:

```bash
dotenv -e ../.env -- dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.Api
```

Fixed lookup data that the code depends on (roles) goes in `HasData` in the configuration plus a migration, not in the seeder.
