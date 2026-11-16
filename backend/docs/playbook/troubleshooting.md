# Troubleshooting: backend

Frontend issues: [frontend troubleshooting](../../../frontend/docs/playbook/troubleshooting.md).

| Symptom | Cause | Fix |
| --- | --- | --- |
| `Connection string 'Default' not found` | The command ran without `.env` | Prefix with `dotenv -e ../.env --` (from `backend/`) |
| `Settings 'Jwt:Key', 'Jwt:Issuer' and 'Jwt:Audience' are required` | Same | Same |
| `Setting 'Seed:…' not found` | A seed setting is missing in `.env` | Add the `Seed__*` variable |
| Cannot connect to SQL Server | The container is not up or not healthy yet | `docker compose up -d db`, `docker compose ps` |
| `address already in use` on 5078 | Another `dotnet run` is still running | Stop it (`Ctrl+C`, or `lsof -i :5078`) |
| Build error: file locked / being used by another process | The API is running while you build or add a migration | Stop the API |
| `Invalid column name 'X'` / `Invalid object name 'X'` | Migration created but not applied | `dotnet ef database update …` |
| New migration contains unexpected drops | Entity changes you did not mean, or the snapshot was edited | `migrations remove`, check `git diff` |
| 500 `Unable to resolve service for type 'IXService'` | Not registered in DI | `builder.Services.AddScoped<…>()` in `Program.cs` |
| 404 on a new endpoint | Wrong `[Route]` / `[HttpGet("…")]`, or the API was not restarted | Check the route in Swagger, restart |
| 401 in Swagger | No token, or it expired (60 min) | Log in again, **Authorize** with the new token |
| 403 | The user's role is not allowed | Log in as Admin, or check `[Authorize(Roles = …)]` |
| 400 with `errors` | DataAnnotations failed | Read `errors`: the field name is the key |
| The new field is always null in responses | Not mapped in `ToXResponse`, or the navigation is missing `Include` | Check the three mapping methods and the repository `Include` |
| Update does not save | Loaded with `AsNoTracking` (`GetByIdAsync`) instead of `GetByIdForUpdateAsync` | Use the tracked method before `SaveChangesAsync` |
| Deleted rows still appear | The new entity lacks `HasQueryFilter(x => x.DeletedAt == null)` | Add it in the configuration |
| Duplicate name allowed after delete is wanted, but blocked | The unique index lacks `.HasFilter("[DeletedAt] IS NULL")` | Add it and create a migration |
| Test project does not compile | A fake does not implement a new interface method | Implement it in `tests/…/Fakes/` |
| Seed rows did not appear | The seeder runs on `database update`, not on `dotnet run` | Run `database update` |

## Docker

| Symptom | Fix |
| --- | --- |
| Ports 8080/4200 busy while running on the host | `docker compose stop api web` |
| Start from a clean database | `docker compose down -v`, then `docker compose up -d db` and `database update` |
| Code changes do not show in the Docker stack | `docker compose up --build` |
| `migrate` service failed | `docker compose logs migrate` |
