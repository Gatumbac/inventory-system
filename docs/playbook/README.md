# Live change playbook

Step-by-step recipes for changing this app under time pressure. Every recipe names the exact files to touch, in order, and uses the real classes of this repo. Copy the closest existing code and adapt it, do not write from memory.

## How to use it

1. Read the request and find the matching recipe below.
2. Open the template files the recipe points to (Suppliers is the simplest full CRUD, Products shows children and attributes).
3. Work inside out: database → backend → Swagger check → frontend → manual check with both roles.
4. Run format and tests before saying it is done.

## Index

| The request sounds like | Backend | Frontend |
| --- | --- | --- |
| "Add a field to products/suppliers" | [backend/add-field.md](backend/add-field.md) | [frontend/add-field.md](frontend/add-field.md) |
| "Add a new entity / CRUD" (categories, warehouses, brands) | [backend/new-resource.md](backend/new-resource.md) | [frontend/new-screen.md](frontend/new-screen.md) |
| "Link X to Y" (product has a category, supplier has contacts) | [backend/relationships.md](backend/relationships.md) | [frontend/dialogs-and-selects.md](frontend/dialogs-and-selects.md) |
| "Filter / sort / search the list by …" | [backend/lists-filters-sorting.md](backend/lists-filters-sorting.md) | [frontend/lists-filters.md](frontend/lists-filters.md) |
| "Validate that …" / "Don't allow …" | [backend/validation-and-rules.md](backend/validation-and-rules.md) | [frontend/forms-and-validation.md](frontend/forms-and-validation.md) |
| "Show a report / total / low stock / cheapest supplier" | [backend/reports-and-queries.md](backend/reports-and-queries.md) | [frontend/report-page.md](frontend/report-page.md) |
| "Only admins can …" / "Add a role" | [backend/auth-and-roles.md](backend/auth-and-roles.md) | [frontend/roles-in-ui.md](frontend/roles-in-ui.md) |
| "Add a number to the dashboard" | [backend/reports-and-queries.md](backend/reports-and-queries.md#existing-examples-to-copy) | [frontend/report-page.md](frontend/report-page.md) |
| "Restore deleted …" / "Add demo data" | [backend/data-lifecycle.md](backend/data-lifecycle.md) | — |
| "Log …" / "Handle this error" | [backend/logging-and-errors.md](backend/logging-and-errors.md) | [frontend/README.md](frontend/README.md#errors-and-notifications) |
| "Change how it looks" | — | [frontend/styles.md](frontend/styles.md) |
| "Add a test" | [backend/testing.md](backend/testing.md) | [frontend/testing.md](frontend/testing.md) |
| Any database change | [backend/migrations.md](backend/migrations.md) | — |

Also:

- [getting-started.md](getting-started.md): run everything on the host.
- [full-stack-walkthrough.md](full-stack-walkthrough.md): one complete change, end to end.
- [troubleshooting.md](troubleshooting.md): symptom → cause → fix.
- [backend/README.md](backend/README.md): where each backend piece lives.
- [frontend/README.md](frontend/README.md): where each frontend piece lives.
- [frontend/angular-for-react-devs.md](frontend/angular-for-react-devs.md): Angular explained in React terms.

## Order of work for a 15–20 minute change

| Minute | Step |
| --- | --- |
| 0–2 | Repeat the request out loud, agree on the scope, name the recipe. Ask which role can do it. |
| 2–8 | Backend: entity/DTO/service/controller, migration if the schema changed. |
| 8–10 | Run the API, try it in Swagger with the Admin user (Authorize button). |
| 10–16 | Frontend: model, service, UI. |
| 16–18 | Manual check as Admin and as Viewer. |
| 18–20 | `dotnet format`, `dotnet test`, `npx ng test --watch=false`, then commit. |

If time runs short, finish the backend with a test and show it in Swagger. A working endpoint with a test is better than a half-done screen.

## Commit

Conventional commits, in English:

```bash
git switch -c feat/<short-name>
git add -A
git commit -m "feat(products): add category to products"
```
