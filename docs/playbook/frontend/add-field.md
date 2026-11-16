# Add a field

**When:** the backend now returns and accepts a new field ([backend/add-field.md](../backend/add-field.md)). Example: `phone` on suppliers.

## Files, in order

### 1. Model: `features/suppliers/supplier.models.ts`

```ts
export interface Supplier {
  id: number;
  name: string;
  description: string | null;
  phone: string | null;
}

export interface SupplierRequest {
  name: string;
  description: string | null;
  phone: string | null;
}
```

The JSON uses camelCase (`Phone` in C# → `phone`). An optional C# `string?` becomes `string | null`.

### 2. Form control: `supplier-form/supplier-form.ts`

```ts
protected readonly form = this.fb.group({
  name: ['', [Validators.required, Validators.maxLength(150)]],
  description: ['', Validators.maxLength(500)],
  phone: ['', [Validators.maxLength(20), Validators.pattern('^[0-9+ -]*$')]],
});
```

Use the same limits as the backend DTO.

Load it in edit mode (`patchValue` in the constructor):

```ts
this.form.patchValue({
  name: supplier.name,
  description: supplier.description ?? '',
  phone: supplier.phone ?? '',
});
```

Send `null` instead of an empty string (`submit`):

```ts
const value = this.form.getRawValue();
const request: SupplierRequest = {
  ...value,
  description: value.description || null,
  phone: value.phone || null,
};
```

Numbers: `minStock: [0, [Validators.required, Validators.min(0)]]`, and `<input matInput type="number">` in the template.

### 3. Form field: `supplier-form/supplier-form.html`

Inside the `<form>`, before `form-actions`:

```html
<mat-form-field appearance="outline">
  <mat-label>Teléfono</mat-label>
  <input matInput formControlName="phone" />
  @if (form.controls.phone.hasError('pattern')) {
    <mat-error>Solo números, espacios, + y -</mat-error>
  } @else if (form.controls.phone.hasError('maxlength')) {
    <mat-error>Máximo 20 caracteres</mat-error>
  }
</mat-form-field>
```

Two fields side by side: wrap them in `<div class="form-grid">` (see `product-form.html`).

### 4. Table column: `supplier-list/supplier-list.ts` + `.html`

Add the column id to the `columns` arrays, in both branches:

```ts
protected readonly columns = this.isAdmin
  ? ['name', 'description', 'phone', 'actions']
  : ['name', 'description', 'phone'];
```

Add the column definition in the table:

```html
<ng-container matColumnDef="phone">
  <th mat-header-cell *matHeaderCellDef>Teléfono</th>
  <td mat-cell *matCellDef="let supplier" class="numeric">{{ supplier.phone ?? '—' }}</td>
</ng-container>
```

The id in `matColumnDef` must match the string in `columns`, or the table throws `Could not find column with id`.

Mobile cards (`<ul class="card-list">`, shown under 720px instead of the table):

```html
@if (supplier.phone) {
  <span class="meta">{{ supplier.phone }}</span>
}
```

### 5. Detail page (products only): `product-detail/product-detail.html`

```html
<div class="detail-field">
  <dt>Unidad</dt>
  <dd>{{ product.unit || '—' }}</dd>
</div>
```

### 6. Specs that build the model

If a spec builds a full object (like `const product: Product = {...}` in `product-list.spec.ts`), add the new property, or TypeScript fails the test build.

## Verify

1. Create and edit: the value is saved and shown again on edit.
2. Break the validation: the `mat-error` shows and the Guardar button does nothing.
3. Narrow the window under 720px: the card shows the field.
4. `npx ng test --watch=false`.
