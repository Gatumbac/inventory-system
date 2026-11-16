# Troubleshooting: frontend

Backend, database and Docker issues: [backend troubleshooting](../../../backend/docs/playbook/troubleshooting.md).

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
