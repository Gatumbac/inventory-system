# Live change playbook: Backend

Step-by-step recipes for changing this app under time pressure. Every recipe names the exact files to touch, in order, and uses the real classes of this repo. Copy the closest existing code and adapt it, do not write from memory.

## How to use it

1. Read the request and find the matching recipe below.
2. Open the template files the recipe points to (Suppliers is the simplest full CRUD, Products shows children and attributes).
3. Work inside out: database → backend → Swagger check → frontend → manual check with both roles.
4. Run format and tests before saying it is done.

## Index

| The request sounds like | Backend recipe | Frontend counterpart |
| --- | --- | --- |
| "Add a field to products/suppliers" | [add-field.md](add-field.md) | [add-field.md](../../../frontend/docs/playbook/add-field.md) |
| "Add a new entity / CRUD" (categories, warehouses, brands) | [new-resource.md](new-resource.md) | [new-screen.md](../../../frontend/docs/playbook/new-screen.md) |
| "Link X to Y" (product has a category, supplier has contacts) | [relationships.md](relationships.md) | [dialogs-and-selects.md](../../../frontend/docs/playbook/dialogs-and-selects.md) |
| "Filter / sort / search the list by …" | [lists-filters-sorting.md](lists-filters-sorting.md) | [lists-filters.md](../../../frontend/docs/playbook/lists-filters.md) |
| "Validate that …" / "Don't allow …" | [validation-and-rules.md](validation-and-rules.md) | [forms-and-validation.md](../../../frontend/docs/playbook/forms-and-validation.md) |
| "Show a report / total / low stock / cheapest supplier" | [reports-and-queries.md](reports-and-queries.md) | [report-page.md](../../../frontend/docs/playbook/report-page.md) |
| "Only admins can …" / "Add a role" | [auth-and-roles.md](auth-and-roles.md) | [roles-in-ui.md](../../../frontend/docs/playbook/roles-in-ui.md) |
| "Add a number to the dashboard" | [reports-and-queries.md](reports-and-queries.md#existing-examples-to-copy) | [report-page.md](../../../frontend/docs/playbook/report-page.md) |
| "Restore deleted …" / "Add demo data" | [data-lifecycle.md](data-lifecycle.md) | — |
| "Log …" / "Handle this error" | [logging-and-errors.md](logging-and-errors.md) | [map.md](../../../frontend/docs/playbook/map.md#errors-and-notifications) |
| "Add a test" | [testing.md](testing.md) | [testing.md](../../../frontend/docs/playbook/testing.md) |
| Any database change | [migrations.md](migrations.md) | — |

Also:

- [getting-started.md](getting-started.md): run the database and the API on the host.
- [full-stack-walkthrough.md](full-stack-walkthrough.md): one complete change, backend half.
- [troubleshooting.md](troubleshooting.md): symptom → cause → fix.
- [map.md](map.md): where each backend piece lives.
- [Frontend playbook](../../../frontend/docs/playbook/README.md): the other half of every recipe.

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
