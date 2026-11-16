# Getting started on the host: backend

For a live change, run SQL Server in Docker and the API and the frontend on the host. Then every save reloads quickly and you can read the logs in the terminal.

The frontend side is in [frontend getting started](../../../frontend/docs/playbook/getting-started.md).

## Ports

| Piece | Host run | Full Docker stack |
| --- | --- | --- |
| Frontend | http://localhost:4200 (`ng serve`) | http://localhost:4200 (nginx) |
| API | http://localhost:5078 | http://localhost:8080 |
| Swagger | http://localhost:5078/swagger | http://localhost:8080/swagger |
| SQL Server | `localhost:${DB_PORT:-1433}` | same |

## Configuration

All settings come from `.env` at the repo root (copy `.env.example`). The API reads them as environment variables, so on the host every `dotnet` command that starts the app goes through `dotenv`:

| Variable | Used for |
| --- | --- |
| `DB_PASSWORD`, `DB_PORT` | SQL Server container |
| `ConnectionStrings__Default` | API and EF tools on the host |
| `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` | token signing (the API refuses to start without them) |
| `Seed__AdminEmail`, `Seed__AdminPassword` | demo Admin user |
| `Seed__ViewerEmail`, `Seed__ViewerPassword` | demo Viewer user |

Never print `.env`. Use the variable names only.

## Start

Terminal 1, database:

```bash
docker compose up -d db
docker compose stop api web
```

`stop api web` frees ports 8080 and 4200 if the full stack was running.

Terminal 2, API (from `backend/`):

```bash
cd backend
dotnet tool restore
dotenv -e ../.env -- dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.Api
dotenv -e ../.env -- dotnet run --project src/Inventory.Api
```

`database update` also runs the demo seeder. Skip it if the database is already up to date.

## Hot reload

Stop with `Ctrl+C` and run again. Or use watch mode:

```bash
dotenv -e ../.env -- dotnet watch --project src/Inventory.Api
```

Watch mode restarts on most changes. If it asks to restart after a "rude edit", accept.

## Check before finishing

```bash
cd backend
dotnet format
dotnet build
dotnet test
```

These are the same checks CI runs (`.github/workflows/ci-cd.yml`).

## Full stack in Docker (alternative)

```bash
docker compose up --build
```

The `migrate` service applies migrations and seeds before the API starts. Use it to show the final result, not to iterate: every change needs a rebuild.
