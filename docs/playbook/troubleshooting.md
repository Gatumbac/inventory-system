# Troubleshooting

## Backend

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

## Frontend

| Symptom | Cause | Fix |
| --- | --- | --- |
| `Could not find the '@angular/build:dev-server' builder's node package` | `node_modules` missing | `npm ci` in `frontend/` |
| Snackbar "No hay conexión con el servidor" | The API is not running, or it runs on another port | Start the API on 5078 |
| CORS error in the console | Frontend not on `http://localhost:4200` | Use `npm start` (port 4200), or add the origin to `Cors:AllowedOrigins` |
| Sent back to login all the time | Token expired → 401 → logout | Log in again |
| `'app-x' is not a known element` / `Can't bind to 'x'` | Component, directive or module missing in `imports: []` | Add it (for example `MatSelectModule`, `RouterLink`, `CurrencyPipe`) |
| `No pipe found with name 'currency'` | Pipe not imported | Add `CurrencyPipe` to `imports` |
| `Could not find column with id "x"` | Id in `columns` with no `matColumnDef="x"` (or a typo) | Make them match |
| The page does not refresh after an action | `load()` not called, or a signal was mutated instead of set | Call `load()`. Use `set`/`update` with a new array |
| Nothing happens on a service call | No `.subscribe()` | Observables are lazy, so subscribe |
| Form submits nothing and shows no error | `form.invalid` and the errors were not touched | `markAllAsTouched()` (already in the forms) |
| `getRawValue()` lacks a field | The control is not in the `fb.group` | Add it |
| Route goes to `/products` | `adminGuard` and the user is not Admin | Expected for Viewer |
| Template change does not show | `ng serve` error in the terminal | Read the terminal. `npx ng build` gives the full error list |
| Spec fails to compile after a model change | Fixture objects miss the new property | Add it to the fixtures |

## Docker

| Symptom | Fix |
| --- | --- |
| Ports 8080/4200 busy while running on the host | `docker compose stop api web` |
| Start from a clean database | `docker compose down -v`, then `docker compose up -d db` and `database update` |
| Code changes do not show in the Docker stack | `docker compose up --build` |
| `migrate` service failed | `docker compose logs migrate` |
