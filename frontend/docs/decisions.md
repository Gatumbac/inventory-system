# Frontend decisions

## Decisions

| Decision | Why |
| --- | --- |
| Angular Material instead of custom components. | Accessible tables, forms, dialogs and snackbar out of the box, and the deadline was one day. |
| Services with signals, no NgRx. | The app is small and I need to explain every piece in the live change. |
| Token and role in `localStorage`. | Simple to follow. The token lasts 60 minutes and there is no refresh, same as the backend. |
| Errors go through one interceptor, logged to the console only. | One place for messages. A backend log endpoint was not worth it for this size. |
| The API is called directly, no dev proxy. | The backend CORS policy already allows `http://localhost:4200`. |
| Lots (price and stock per supplier) are managed in the product detail, not in the edit form. | One screen works for both roles and creating a product stays a single step. |
| Supplier picker is an autocomplete with 300 ms debounce, server search and a "Cargar más…" button. | The list can grow, so it should not load everything. |
| Delete asks for confirmation with a Material dialog (`ConfirmService`). | The native `confirm()` ignored the theme and did not look destructive. One call works for any future delete. |
| List attributes show the first two plus `+N`. | Rows had different heights, and the full list is still in the tooltip and the detail page. |
| Own `.panel-*` classes, not `.card-*`. | `.card` names were already used by the mobile list view. |
| Admin actions are hidden for Viewer and the routes are also guarded. | Hiding a button is not security, the backend returns 403 anyway. |

## Challenges

| Problem | Solution |
| --- | --- |
| The row chevron sat 3 px above the row menu. | `mat-icon` sets `display: inline-block` with the same specificity, so my `display: block` never applied. Moved the rule under `.data-table .cell-chevron`. Measured in the browser, both icons now share the same center. |
| The attribute column made rows grow to about 100 px. | One line with ellipsis and a `title`. |
| Tests failed because the API URL was different from what I expected. | In tests `environment.apiUrl` is the full `http://localhost:5078/api`, so specs build URLs from it. |

## Pending

| Item | Note |
| --- | --- |
| No Dockerfile or nginx for the frontend, and it is not in `docker-compose.yml`. | The production `apiUrl` of `/api` assumes a reverse proxy that does not exist yet. |
| Specs cover guards, services, the product list and shared pieces. | Forms, product detail and the supplier list have no component tests. |
| Empty states are repeated markup. | A shared `EmptyState` component would remove the duplication. |
