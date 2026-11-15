# Backend conventions

Rules followed across the API. Layering rules are in `architecture/architecture.md`.

## Request flow

- Controller (`Inventory.Api`) -> service (`Inventory.Core/Services`) -> repository (`Inventory.Infrastructure/Persistence`).
- Interfaces live in `Core/Abstractions`, DTOs in `Core/Dtos`, results in `Core/Results`.
- Services map entities to DTOs with private methods. No mapping library.
- Concrete types are registered only in `Program.cs`, one line per interface.

## Results and errors

- Expected business outcomes (not found, duplicate, has children) are returned as a typed result: a status enum, plus a payload when saving. Result types are classes, not records.
- The controller translates results to HTTP. Exceptions are only for the unexpected and are handled by `GlobalExceptionHandler`.
- User-facing messages are Spanish constants in `Core/Constants/ErrorMessages.cs`. Everything else (code, logs, docs, commits) is English.
- 404 and 409 responses use `Problem(statusCode, title: ErrorMessages.X)` so the client always reads `title`.

## Validation

- DataAnnotations on request DTOs, with `ErrorMessages` constants for custom messages. `[ApiController]` returns 400 automatically.
- Text inputs are trimmed in the service before comparing or saving.
- Cross-field rules use `IValidatableObject`.

## Data

- Soft delete: nullable `DeletedAt`, `HasQueryFilter(x => x.DeletedAt == null)` in the entity configuration, and `RemoveAsync` sets `DeletedAt`.
- Unique indexes are filtered with `[DeletedAt] IS NULL`, so deleted names and codes can be reused.
- Soft delete never triggers the `Restrict` foreign key, so repositories check for active children before deleting a parent.
- `CreatedAt` and `UpdatedAt` are stamped by `AuditSaveChangesInterceptor` on any entity that has both properties.
- Reads use `AsNoTracking`. Updates load the entity with a separate tracked method (`GetByIdForUpdateAsync`).
- A null lot counts as a value when checking duplicates, like the SQL Server unique index.
- The seeder runs with `dotnet ef database update`, not at API startup.

## Pagination

- List endpoints inherit `PageRequest` (`page`, `pageSize`) and return `PagedResult<T>`.
- The repository orders the query (always ending in `Id`) and calls `PagedQuery.ToPagedAsync`. Use `PagedResult<T>.Map` in the service to convert items.
- Offset pagination, chosen because the UI needs numbered pages.

## Code style

- No comments. If code needs one, rewrite the code.
- Objects are created with `new X()` and a multi-line initializer, never on one line.
- A newly built object is assigned to a local and then returned: `var result = new X() { ... }; return result;`.
- Methods stay short. Split into private helpers instead of growing one method.
- No new dependencies without discussion.

## Before finishing a change

- `dotnet format Inventory.slnx`, then `dotnet build Inventory.slnx` with zero warnings.
- There is no test project, so `dotnet test` runs nothing. Endpoints are checked with a manual smoke test against a second API instance.
