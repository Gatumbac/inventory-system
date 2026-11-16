# Frontend map

Angular 22, standalone components, Angular Material, signals for state, RxJS for HTTP. New to Angular? Read [angular-for-react-devs.md](angular-for-react-devs.md) first.

```
frontend/src/
  app/
    app.config.ts      providers: router, HttpClient + interceptors
    app.routes.ts      all routes, lazy loaded, guards
    core/              app-wide singletons
      auth/            AuthService, authGuard, adminGuard, authInterceptor
      http/            errorInterceptor, PagedResult<T>
      layout/          Shell (side menu + <router-outlet>), SpanishPaginatorIntl
      logging/         LoggerService
      notifications/   NotificationService (snackbar)
    features/          one folder per area
      login/
      dashboard/       home page (default route): summary cards and low-stock lots
      products/        list, form, detail, lot dialog, price matrix, services, models
      suppliers/       list, form, supplier-select (autocomplete), service, models
      users/           admin only: list (active/inactive), form, reactivate
    shared/            reusable UI: search-field, row-actions, skeleton-rows, confirm-dialog
  environments/        apiUrl: dev → http://localhost:5078/api, prod → /api
  styles.scss          Material theme, tokens, shared classes
```

## Suppliers as the template

| # | Piece | File |
| --- | --- | --- |
| 1 | Types | `features/suppliers/supplier.models.ts` (`Supplier`, `SupplierRequest`) |
| 2 | API client | `features/suppliers/supplier.service.ts` (`list`, `get`, `create`, `update`, `remove`) |
| 3 | List page | `features/suppliers/supplier-list/supplier-list.ts` + `.html` |
| 4 | Create/edit page | `features/suppliers/supplier-form/supplier-form.ts` + `.html` |
| 5 | Routes | `app.routes.ts` (`suppliers`, `suppliers/new`, `suppliers/:id/edit`) |
| 6 | Menu link | `core/layout/shell.html` |

Products adds a detail page (`product-detail`), a dialog (`product-supplier-dialog`) and a nested service (`product-supplier.service.ts`).

Other templates:

| Need | Copy |
| --- | --- |
| Admin-only screen (route with `adminGuard`, menu link inside `@if (auth.isAdmin())`) | `features/users` |
| List with a two-state filter and a non-delete row action | `features/users/user-list` (Activos/Inactivos, reactivate) |
| Summary cards + small table | `features/dashboard` |
| Pivot table built from an API response | `features/products/price-matrix` |

`''` redirects to `dashboard`. `adminGuard` still sends non-admins to `/products`.

## Reuse before writing

| Need | Use | Where |
| --- | --- | --- |
| Search box with debounce | `<app-search-field label="…" (search)="onSearch($event)" />` | `shared/search-field` |
| Edit/delete menu per row | `<app-row-actions [label]="x.name" [editLink]="[...]" (remove)="remove(x)" />` or `(edit)="…"` for dialogs | `shared/row-actions` |
| Loading placeholder rows | `<app-skeleton-rows [widths]="[30, 55]" />` | `shared/skeleton-rows` |
| "Are you sure?" | `confirm.ask({ title, message, confirmLabel, destructive })` → `Observable<boolean>` | `shared/confirm-dialog/confirm.service.ts` |
| Success toast | `notifications.success('…')` | `core/notifications/notification.service.ts` |
| Paged response type | `PagedResult<T>` | `core/http/paged-result.ts` |
| Paginator in Spanish | `providers: [{ provide: MatPaginatorIntl, useClass: SpanishPaginatorIntl }]` | `core/layout/paginator-intl.ts` |
| Supplier picker | `<app-supplier-select [control]="form.controls.supplier" />` + `supplierSelected` validator | `features/suppliers/supplier-select` |
| Role check | `inject(AuthService).isAdmin()` | `core/auth/auth.service.ts` |

## Errors and notifications

Components **do not** show error messages. `core/http/error.interceptor.ts` catches every HTTP error, logs it, and shows a Spanish snackbar:

| Status | Message |
| --- | --- |
| 0 | No hay conexión con el servidor |
| 400 | First message from `errors`, or "Datos inválidos" |
| 401 | "Credenciales inválidas" on login, otherwise "Sesión expirada" + logout + `/login` |
| 403 | No tienes permiso para esta acción |
| other | The API's `title`, or "Error inesperado" |

So in components:

```ts
.pipe(catchError(() => EMPTY))
```

just stops the stream. Use `notifications.success(...)` for good outcomes only.

To add a special message for a new status (for example 422), add a `case` in `messageFor` in `error.interceptor.ts`. The backend already sends the Spanish text in `title`, so the `default` branch usually covers it.

## Commands

From `frontend/`:

```bash
npm ci
npm start
npx ng test --watch=false
npx ng build
npx prettier --write src
```

Generate a component (scss is the default in `angular.json`):

```bash
npx ng g c features/categories/category-list
```

It creates `category-list.ts`, `.html`, `.scss` and `.spec.ts`, with the class `CategoryList`. Copying the Suppliers files is usually faster because they already have the signals, the table and the paginator.

## Recipes

- [add-field.md](add-field.md)
- [new-screen.md](new-screen.md)
- [lists-filters.md](lists-filters.md)
- [forms-and-validation.md](forms-and-validation.md)
- [dialogs-and-selects.md](dialogs-and-selects.md)
- [report-page.md](report-page.md)
- [roles-in-ui.md](roles-in-ui.md)
- [styles.md](styles.md)
- [testing.md](testing.md)
