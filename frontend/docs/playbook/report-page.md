# Report or read-only page

**When:** the backend has a report endpoint ([backend/reports-and-queries.md](../../../backend/docs/playbook/reports-and-queries.md)): stock per product, low stock, inventory value, a dashboard.

Two working examples are already in the app:

- `features/dashboard/`: summary cards and a low-stock table from `GET api/dashboard`. To add a number, add the field to `dashboard.models.ts` and a card to `dashboard.html`.
- `features/products/price-matrix/`: a paged pivot table (products × suppliers) from `GET api/products/price-matrix`.

Start from the closest one. The steps below are for a report that fits neither.

## 1. Model and service

Put them next to the feature they belong to (`features/products/`), or in a new `features/reports/`.

```ts
export interface ProductStock {
  productId: number;
  code: string;
  name: string;
  totalStock: number;
  minPrice: number | null;
  supplierCount: number;
}
```

```ts
stock(page: number, pageSize: number, search: string) {
  const params = this.pageParams(page, pageSize, search);
  return this.http.get<PagedResult<ProductStock>>(`${this.url}/stock`, { params });
}
```

For a report without pagination (the backend returns a plain array):

```ts
stockAll() {
  return this.http.get<ProductStock[]>(`${this.url}/stock`);
}
```

## 2. Component

Paged → copy `supplier-list` and remove everything about actions (`remove`, `ConfirmService`, `RowActions`, the "Nuevo" button, the `actions` column).

Not paged:

```ts
@Component({
  selector: 'app-stock-report',
  imports: [CurrencyPipe, DecimalPipe, MatTableModule, SkeletonRows],
  templateUrl: './stock-report.html',
  host: { class: 'page' },
})
export class StockReport {
  private readonly service = inject(ProductService);

  protected readonly columns = ['code', 'name', 'totalStock', 'minPrice'];
  protected readonly rows = signal<ProductStock[]>([]);
  protected readonly loading = signal(true);
  protected readonly totalStock = computed(() =>
    this.rows().reduce((sum, row) => sum + row.totalStock, 0),
  );

  constructor() {
    this.service
      .stockAll()
      .pipe(
        catchError(() => EMPTY),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((rows) => this.rows.set(rows));
  }
}
```

`computed` recalculates when `rows` changes (it is like `useMemo` with automatic dependencies).

## 3. Template

```html
<header class="page-header">
  <h1>Stock por producto</h1>
</header>

<div class="data-surface">
  <table mat-table class="data-table" [dataSource]="rows()">
    <ng-container matColumnDef="code">
      <th mat-header-cell *matHeaderCellDef>Código</th>
      <td mat-cell *matCellDef="let row" class="numeric">{{ row.code }}</td>
    </ng-container>

    <ng-container matColumnDef="name">
      <th mat-header-cell *matHeaderCellDef>Nombre</th>
      <td mat-cell *matCellDef="let row" class="cell-name">{{ row.name }}</td>
    </ng-container>

    <ng-container matColumnDef="totalStock">
      <th mat-header-cell *matHeaderCellDef class="cell-numeric">Stock</th>
      <td mat-cell *matCellDef="let row" class="cell-numeric">{{ row.totalStock | number }}</td>
    </ng-container>

    <ng-container matColumnDef="minPrice">
      <th mat-header-cell *matHeaderCellDef class="cell-numeric">Mejor precio</th>
      <td mat-cell *matCellDef="let row" class="cell-numeric">
        {{ row.minPrice === null ? '—' : (row.minPrice | currency) }}
      </td>
    </ng-container>

    <tr mat-header-row *matHeaderRowDef="columns"></tr>
    <tr mat-row *matRowDef="let row; columns: columns"></tr>
  </table>

  @if (loading()) {
    <app-skeleton-rows [widths]="[12, 40, 10, 12]" />
  }
</div>

<p class="meta">Stock total: {{ totalStock() | number }}</p>
```

Highlight a row (low stock):

```html
<tr mat-row *matRowDef="let row; columns: columns" [class.low-stock]="row.totalStock < 5"></tr>
```

```scss
.low-stock td {
  color: var(--mat-sys-error);
}
```

## 4. Route and menu

Route inside the shell `children` (no `adminGuard` unless only admins may see it), and a link in `core/layout/shell.html` (icon `bar_chart` or `analytics`). See [new-screen.md](new-screen.md#2-routes-approutests).

## Pipes

Pipes are formatting functions used in templates. Import each one in the component `imports`:

| Pipe | Import | Example | Output |
| --- | --- | --- | --- |
| `currency` | `CurrencyPipe` | `{{ 750 \| currency }}` | `$750.00` (USD, as in the product detail) |
| `number` | `DecimalPipe` | `{{ 1234.5 \| number: '1.0-2' }}` | `1,234.5` |
| `percent` | `PercentPipe` | `{{ 0.25 \| percent }}` | `25%` |
| `date` | `DatePipe` | `{{ row.createdAt \| date: 'dd/MM/yyyy HH:mm' }}` | `16/11/2026 15:15` |

The app does not register a Spanish locale, so numbers use the `en-US` format. If asked for `1.234,50`, register `es` (`registerLocaleData(localeEs)` + `{ provide: LOCALE_ID, useValue: 'es' }` in `app.config.ts`), but that changes every number in the app, so say so first.

## Dashboard cards

For a few single numbers, use panels instead of a table:

```html
<div class="form-grid">
  <section class="panel">
    <div class="panel-body">
      <span class="meta">Productos</span>
      <h2>{{ summary()?.products ?? '—' }}</h2>
    </div>
  </section>
</div>
```

`.panel`, `.panel-body` and `.form-grid` (two columns, one on mobile) are already in `styles.scss`.
