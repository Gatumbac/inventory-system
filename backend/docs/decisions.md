# Backend decisions

## Decisions

| Decision | Why |
| --- | --- |
| Clean Architecture in three projects (Core, Infrastructure, Api), with controllers instead of Minimal APIs. | Follows the Microsoft book. Core has no dependencies and the compiler enforces the layers. |
| `AuthService` lives in Core, behind repository and token interfaces. | Putting it in Infrastructure with the DbContext was evaluated and dropped. The book places domain services in Core. |
| Expected cases (not found, duplicate code) come back as typed results. Exceptions are only for real failures. | The controller maps each result to 404 or 409, and the service stays easy to test. |
| Soft delete through `DeletedAt` on Product, Supplier, ProductSupplier and User. | A global filter hides deleted rows and unique indexes ignore them, so a code can be reused. |
| `CreatedAt` and `UpdatedAt` are set in a `SaveChanges` interceptor. | EF has no built-in attribute for it. |
| Product `Code` is typed by the user, normalized, and unique among active rows. | A duplicate returns 409. |
| Price and stock per supplier and lot live in `ProductSupplier`. | Unique index on product, supplier and lot. |
| Offset pagination on the server (`page`, `pageSize`). | The UI shows numbered pages. Reads use `AsNoTracking`. |
| JWT with Admin and Viewer roles. Token lasts 60 minutes, no refresh token. | Admin writes, Viewer only reads. The missing refresh token is a known limitation. |
| Migrations in Docker run as a bundle in a separate `migrate` service. | Preferred over `Migrate()` on startup so `Program.cs` and the `dotnet run` flow stay untouched. |

## Challenges

| Problem | Solution |
| --- | --- |
| Dev Container did not work and was slow for the live-change part. | .NET SDK installed in the user folder, Docker only for SQL Server. |
| Global `dotnet-ef` 11 does not run on SDK 10, and `dotnet add package` without a version fails with NU1202. | Local tool pinned to 10.0.15 in `dotnet-tools.json`, and packages pinned to the same version. |
| EF adds `WHERE [col] IS NOT NULL` to unique indexes on nullable columns, so a duplicate (product, supplier, NULL lot) went through. | `.HasFilter(null)` on that index. The duplicate error was confirmed (Msg 2601). |
| Soft delete needs filtered unique indexes, and `sqlcmd` fails on them without `-I`. | Indexes filtered by `DeletedAt IS NULL`, and `-I` in the SQL checks. |
| A required unique column breaks a table that already has rows. | Migrations squashed into one `InitialCreate` and the local database recreated. |
| The seeder only runs with `dotnet ef database update`, not when the API starts, and it uses sync `SaveChanges`. | Reseed by running the update, and the audit interceptor covers sync and async. |
| `[Authorize(Roles = ...)]` depends on how the role claim is mapped. | `MapInboundClaims = false` and `RoleClaimType = "role"`. Viewer gets 403 on writes. |
| The built-in OpenAPI package only generates the document, and Swashbuckle 11 changed how the Bearer scheme is declared. | Swashbuckle 11 with `OpenApiSecuritySchemeReference`. |
| `required` in `JwtSettings` is not enforced by the config binder. | `Program.cs` checks key, issuer and audience at startup. Side effect: `dotnet ef` needs the `Jwt__*` variables. |
| An old `dotnet run` on port 5078 answered instead of the new build. | New builds are tested on another port. |
| Compose services only get variables declared in `environment` or `env_file`. The `.env` connection string points to `localhost`. | The `api` and `migrate` services redefine it with `Server=db`. |
| The migration bundle needs config at build time, and Serilog cannot find its sinks in a single-file executable. | Placeholder values only for that build step, and `Serilog__Using__*` variables on the `migrate` service. |

## Pending

| Item | Note |
| --- | --- |
| `GET /api/products` loads the full entity with its attributes. | With more data it would make sense to project to a DTO with `Select`. |
| Endpoint behavior (roles, validation, pagination limits) is checked by hand. | Unit tests only cover the services. |
