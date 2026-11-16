# Live change playbook: Frontend

Step-by-step recipes for changing this app under time pressure. Every recipe names the exact files to touch, in order, and uses the real classes of this repo. Copy the closest existing code and adapt it, do not write from memory.

## How to use it

1. Read the request and find the matching recipe below.
2. Open the template files the recipe points to (Suppliers is the simplest full CRUD, Products shows children and attributes).
3. Work inside out: database → backend → Swagger check → frontend → manual check with both roles.
4. Run format and tests before saying it is done.

## Index

| The request sounds like | Frontend recipe | Backend counterpart |
| --- | --- | --- |
| "Add a field to products/suppliers" | [add-field.md](add-field.md) | [add-field.md](../../../backend/docs/playbook/add-field.md) |
| "Add a new entity / CRUD" (categories, warehouses, brands) | [new-screen.md](new-screen.md) | [new-resource.md](../../../backend/docs/playbook/new-resource.md) |
| "Link X to Y" (product has a category, supplier has contacts) | [dialogs-and-selects.md](dialogs-and-selects.md) | [relationships.md](../../../backend/docs/playbook/relationships.md) |
| "Filter / sort / search the list by …" | [lists-filters.md](lists-filters.md) | [lists-filters-sorting.md](../../../backend/docs/playbook/lists-filters-sorting.md) |
| "Validate that …" / "Don't allow …" | [forms-and-validation.md](forms-and-validation.md) | [validation-and-rules.md](../../../backend/docs/playbook/validation-and-rules.md) |
| "Show a report / total / low stock / cheapest supplier" | [report-page.md](report-page.md) | [reports-and-queries.md](../../../backend/docs/playbook/reports-and-queries.md) |
| "Only admins can …" / "Add a role" | [roles-in-ui.md](roles-in-ui.md) | [auth-and-roles.md](../../../backend/docs/playbook/auth-and-roles.md) |
| "Add a number to the dashboard" | [report-page.md](report-page.md) | [reports-and-queries.md](../../../backend/docs/playbook/reports-and-queries.md#existing-examples-to-copy) |
| "Log …" / "Handle this error" | [map.md](map.md#errors-and-notifications) | [logging-and-errors.md](../../../backend/docs/playbook/logging-and-errors.md) |
| "Change how it looks" | [styles.md](styles.md) | — |
| "Add a test" | [testing.md](testing.md) | [testing.md](../../../backend/docs/playbook/testing.md) |

Also:

- [getting-started.md](getting-started.md): run the frontend on the host.
- [full-stack-walkthrough.md](full-stack-walkthrough.md): one complete change, frontend half.
- [troubleshooting.md](troubleshooting.md): symptom → cause → fix.
- [map.md](map.md): where each frontend piece lives.
- [angular-for-react-devs.md](angular-for-react-devs.md): Angular explained in React terms.
- [Backend playbook](../../../backend/docs/playbook/README.md): the other half of every recipe.

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
