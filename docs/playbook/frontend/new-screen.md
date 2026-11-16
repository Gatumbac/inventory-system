# New CRUD screen

**When:** the backend has a new resource ([backend/new-resource.md](../backend/new-resource.md)). Example: categories at `api/categories`.

## 1. Copy and rename

From `frontend/src/app/features/`:

```bash
mkdir -p categories/category-list categories/category-form
cp suppliers/supplier.models.ts                 categories/category.models.ts
cp suppliers/supplier.service.ts                categories/category.service.ts
cp suppliers/supplier-list/supplier-list.ts     categories/category-list/category-list.ts
cp suppliers/supplier-list/supplier-list.html   categories/category-list/category-list.html
cp suppliers/supplier-form/supplier-form.ts     categories/category-form/category-form.ts
cp suppliers/supplier-form/supplier-form.html   categories/category-form/category-form.html
```

Replace inside the new files, matching case:

| Find | Replace |
| --- | --- |
| `Supplier` | `Category` |
| `supplier` | `category` |
| `suppliers` | `categories` (URLs: `/categories`, `${environment.apiUrl}/categories`) |
| `Proveedores` / `proveedor` / `Proveedor` | `Categorías` / `categoría` / `Categoría` |

Check by hand:

- Spanish texts: titles, empty states, buttons, snackbar (`'Categoría guardada'`, `'Categoría eliminada'`), confirm dialog.
- Gender in Spanish: "Nueva categoría", "Aún no hay categorías", "Crea la primera…".
- Selectors: `app-category-list`, `app-category-form`.
- Validators match the backend DTO.

## 2. Routes: `app.routes.ts`

Inside the `children` of the shell route (so they get the menu and `authGuard`):

```ts
{
  path: 'categories',
  loadComponent: () =>
    import('./features/categories/category-list/category-list').then((m) => m.CategoryList),
},
{
  path: 'categories/new',
  canActivate: [adminGuard],
  loadComponent: () =>
    import('./features/categories/category-form/category-form').then((m) => m.CategoryForm),
},
{
  path: 'categories/:id/edit',
  canActivate: [adminGuard],
  loadComponent: () =>
    import('./features/categories/category-form/category-form').then((m) => m.CategoryForm),
},
```

`categories/new` goes before `categories/:id/...`. Order matters only when two paths could match the same URL, but keeping literals first is a safe habit.

## 3. Menu: `core/layout/shell.html`

Inside `<nav>`:

```html
<a routerLink="/categories" routerLinkActive="active" (click)="navOpen.set(false)">
  <mat-icon fontSet="material-symbols-outlined">category</mat-icon>
  Categorías
</a>
```

Icon names come from Material Symbols (fonts.google.com/icons): `category`, `warehouse`, `sell`, `bar_chart`, `group`, `inventory`, `local_offer`.

## 4. Only some parts

| Request | Do |
| --- | --- |
| Read-only list | Copy only models, service (`list`) and list. Drop the `actions` column, `remove`, the "Nuevo" button and the form routes. |
| Detail page | Copy `features/products/product-detail` (panel with `detail-fields`). Route `categories/:id`, without `adminGuard`. |
| Edit in a dialog instead of a page | See [dialogs-and-selects.md](dialogs-and-selects.md). |
| No pagination | Service returns `this.http.get<Category[]>(this.url)`. Remove the paginator, `page`, `pageSize` and `totalCount`. |

## Verify

1. As Admin: menu link → list → Nuevo → save → snackbar → back in the list. Edit, delete with confirmation.
2. As Viewer: no Nuevo button, no row menu, `/categories/new` typed by hand redirects to `/products`.
3. Search and paginator work (if the backend supports them).
4. `npx ng build` (catches template errors that `ng serve` shows only in the browser).
