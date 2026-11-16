# Migrations

Every schema change (new entity, new column, new index, changed length) needs a migration. Run all commands from `backend/`, with the database container up, through `dotenv` so the tools see `ConnectionStrings__Default` and the JWT settings (`Program.cs` checks them on startup, and the EF tools run `Program.cs`).

## Setup once

```bash
cd backend
dotnet tool restore
docker compose up -d db
```

`dotnet-ef` is a local tool pinned in `backend/dotnet-tools.json`.

To keep the commands short, set these in your shell:

```bash
EF_ARGS="--project src/Inventory.Infrastructure --startup-project src/Inventory.Api"
```

## Commands

| Goal | Command |
| --- | --- |
| Create a migration | `dotenv -e ../.env -- dotnet ef migrations add AddSupplierPhone $EF_ARGS --output-dir Persistence/Migrations` |
| Apply to the database | `dotenv -e ../.env -- dotnet ef database update $EF_ARGS` |
| List (shows Pending) | `dotenv -e ../.env -- dotnet ef migrations list $EF_ARGS` |
| Remove the last one, **not applied yet** | `dotenv -e ../.env -- dotnet ef migrations remove $EF_ARGS` |
| Undo an **applied** one | `dotenv -e ../.env -- dotnet ef database update InitialCreate $EF_ARGS`, then `migrations remove` |
| See the SQL | `dotenv -e ../.env -- dotnet ef migrations script $EF_ARGS --idempotent` |
| Drop the database | `dotenv -e ../.env -- dotnet ef database drop $EF_ARGS --force` |
| Wipe the Docker volume | `docker compose down -v` (deletes `db-data`, then `up -d db` and `database update`) |

Name migrations in PascalCase and describe the change: `AddProductCategory`, `AddSupplierPhone`, `AddWarehouses`.

To undo an applied migration, `database update <Name>` takes the name of the migration you want to **go back to**. Today the only one is `InitialCreate`.

## The workflow

1. Change the entity and its `Configuration`.
2. Stop the API. A running API locks the DLLs and the build inside `migrations add` fails.
3. `migrations add <Name>`.
4. **Read the generated file** in `Persistence/Migrations/`. Look for `DropColumn`/`DropTable` you did not expect, and check that new required columns on existing tables have a default.
5. `database update`.
6. Start the API again.

## Adding a required column to a table with data

SQL Server cannot add a `NOT NULL` column to a table with rows without a default. EF generates `defaultValue: ""` or `0` for you, and the existing rows get it. If that default is wrong:

- make the column nullable (`string?`), or
- set a real default in the configuration: `builder.Property(p => p.Status).HasDefaultValue("Active");`

## Seeding

`DbSeeder.Seed` is wired through `UseSeeding` in `Program.cs`, so it runs on every `database update` and in the Docker `migrate` bundle. Every `Ensure*` helper checks first, so running it twice is safe. After you add seed rows, run `database update` again (even with no pending migration). See [data-lifecycle.md](data-lifecycle.md#seed-data).

Fixed lookup rows (like the roles in `RoleConfiguration`) use `HasData` instead and need a migration.

## In Docker

`docker compose up --build` builds an EF migrations bundle (`backend/Dockerfile`, stage `bundle`). The `migrate` service runs it before the API starts. New migrations are picked up on the next `--build`. Nothing else to do.

## Gotchas

| Symptom | Fix |
| --- | --- |
| `Connection string 'Default' not found` | You forgot `dotenv -e ../.env --`. |
| `Settings 'Jwt:Key'… are required` | Same: the EF tools run `Program.cs`. |
| Build fails, file is locked by another process | Stop `dotnet run` / `dotnet watch`. |
| `Invalid column name 'X'` at runtime | The migration was created but not applied: `database update`. |
| `The migration … has already been applied` on remove | Revert first with `database update <Previous>`. |
| Migration contains changes you did not make | The snapshot was out of date. Remove the migration and check `git status` for stray entity changes. |
| Cannot connect | `docker compose up -d db` and wait for it to be healthy (`docker compose ps`). |
