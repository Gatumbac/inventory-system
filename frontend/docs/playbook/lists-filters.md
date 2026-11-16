# List filters, search and sorting

How a list works today (`product-list.ts`, `supplier-list.ts`):

- State in signals: `page`, `pageSize`, `search`, `totalCount`, `loading`, `failed`, and the items.
- `load()` calls `service.list(page, pageSize, search)` and fills the signals.
- Any change in criteria → set the signal → `page.set(1)` → `load()`.

**A working example already exists:** `features/users/user-list` filters active/inactive users with a `mat-button-toggle-group` → `inactive` signal → `onStatus()` → `page.set(1)` → `load()`, and `user.service.ts` sends `inactive` as a query param. Backend side: `UserFilterRequest.Inactive` + `UserRepository.GetPagedAsync`. Copy it for any two-state filter.

## A. Add a filter (dropdown)

**When:** the backend accepts `?supplierId=` ([backend/lists-filters-sorting.md](../../../backend/docs/playbook/lists-filters-sorting.md)).

### 1. Service: `product.service.ts`

```ts
list(page: number, pageSize: number, search: string, supplierId: number | null = null) {
  const base = this.pageParams(page, pageSize, search);
  const params = supplierId === null ? base : base.set('supplierId', supplierId);
  return this.http.get<PagedResult<Product>>(this.url, { params });
}
```

`pageParams` is the private helper already in `product.service.ts` (page, pageSize, optional search). `HttpParams` is immutable, so `set` returns a new object.

The default value keeps the existing calls and specs working.

### 2. Component: `product-list.ts`

```ts
protected readonly supplierId = signal<number | null>(null);
protected readonly suppliers = signal<Supplier[]>([]);

constructor() {
  this.supplierService
    .list(1, 50, '')
    .pipe(catchError(() => EMPTY))
    .subscribe((result) => this.suppliers.set(result.items));
  this.load();
}

protected onSupplier(id: number | null): void {
  this.supplierId.set(id);
  this.page.set(1);
  this.load();
}
```

In `load()`:

```ts
.list(this.page(), this.pageSize(), this.search(), this.supplierId())
```

Add `private readonly supplierService = inject(SupplierService);`, and add `MatFormFieldModule` and `MatSelectModule` to `imports`.

The page size is capped at 50 by the backend. For more options, use the `SupplierSelect` autocomplete instead ([dialogs-and-selects.md](dialogs-and-selects.md)).

### 3. Template: `product-list.html`

Next to the search field:

```html
<div class="filters">
  <app-search-field label="Buscar por código o nombre" (search)="onSearch($event)" />
  <mat-form-field appearance="outline" subscriptSizing="dynamic">
    <mat-label>Proveedor</mat-label>
    <mat-select [value]="supplierId()" (selectionChange)="onSupplier($event.value)">
      <mat-option [value]="null">Todos</mat-option>
      @for (supplier of suppliers(); track supplier.id) {
        <mat-option [value]="supplier.id">{{ supplier.name }}</mat-option>
      }
    </mat-select>
  </mat-form-field>
</div>
```

Add a small style in the component scss, or in `styles.scss` if it is reused:

```scss
.filters {
  display: flex;
  flex-wrap: wrap;
  gap: var(--app-space-3);
  align-items: flex-start;
}
```

### 4. Empty state

The empty state checks `search()`. With filters, show "Sin resultados" when any filter is set:

```html
@if (search() || supplierId() !== null) {
```

## B. Toggle filter (checkbox / slide toggle)

```html
<mat-slide-toggle [checked]="lowStock()" (change)="onLowStock($event.checked)">Solo stock bajo</mat-slide-toggle>
```

`MatSlideToggleModule` in `imports`. Same pattern: signal → `page.set(1)` → `load()`.

## C. Sorting with the table headers

Backend with `sortBy` and `descending` ([backend/lists-filters-sorting.md](../../../backend/docs/playbook/lists-filters-sorting.md#c-sorting-chosen-by-the-client)).

```html
<table mat-table matSort (matSortChange)="onSort($event)" …>
  <th mat-header-cell *matHeaderCellDef mat-sort-header="name">Nombre</th>
```

```ts
protected onSort(sort: Sort): void {
  this.sortBy.set(sort.direction ? sort.active : '');
  this.descending.set(sort.direction === 'desc');
  this.page.set(1);
  this.load();
}
```

`MatSortModule` in `imports`. `Sort` comes from `@angular/material/sort`. Send `sortBy`/`descending` as `HttpParams` only when `sortBy` is set.

## D. Sorting or filtering on the client only

For short lists already in memory (lots of a product), use a `computed`:

```ts
protected readonly sortedLots = computed(() =>
  [...this.lots()].sort((a, b) => a.price - b.price),
);
```

Then use `[dataSource]="sortedLots()"`.

## E. Keep filters in the URL (optional)

Only if asked ("share the filtered link"). Read with `inject(ActivatedRoute).snapshot.queryParamMap.get('supplierId')`, and write with `router.navigate([], { queryParams: { supplierId }, queryParamsHandling: 'merge' })`.

## Verify

- Change the filter: the list reloads from page 1 and the paginator total updates.
- Filter + search together.
- "Todos" clears the filter.
