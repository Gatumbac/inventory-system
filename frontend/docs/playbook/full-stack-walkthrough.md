# Full-stack walkthrough, frontend half: add a brand to products

A complete example of the most likely kind of live change: one new field, end to end, searchable. Products have the most moving parts (attributes, code normalization), so if this works, the other recipes are smaller.

Request: *"Products need an optional brand (max 100 characters), shown in the list and searchable."*

Time target: about 15 minutes.

## 0. Before typing

- Say the plan: entity → migration → DTOs → service → search → test → Swagger → frontend model → form → list.
- Do the backend half first: [backend walkthrough](../../../backend/docs/playbook/full-stack-walkthrough.md).
- Start the stack ([getting-started.md](getting-started.md)), and create a branch:

```bash
git switch -c feat/product-brand
```

## 2. Frontend (from `frontend/src/app/features/products/`)

### Model: `product.models.ts`

```ts
brand: string | null;
```

Add it to both `Product` and `ProductRequest`.

### Form: `product-form/product-form.ts`

```ts
brand: ['', Validators.maxLength(100)],
```

In `patchValue`: `brand: product.brand ?? '',`

In `submit`:

```ts
const request: ProductRequest = {
  ...value,
  description: value.description || null,
  brand: value.brand || null,
};
```

### Form field: `product-form/product-form.html`

Below the code/name grid:

```html
<mat-form-field appearance="outline">
  <mat-label>Marca</mat-label>
  <input matInput formControlName="brand" />
  @if (form.controls.brand.hasError('maxlength')) {
    <mat-error>Máximo 100 caracteres</mat-error>
  }
</mat-form-field>
```

### List: `product-list/product-list.ts` + `.html`

```ts
protected readonly columns = this.isAdmin
  ? ['code', 'name', 'brand', 'description', 'attributes', 'actions', 'chevron']
  : ['code', 'name', 'brand', 'description', 'attributes', 'chevron'];
```

```html
<ng-container matColumnDef="brand">
  <th mat-header-cell *matHeaderCellDef>Marca</th>
  <td mat-cell *matCellDef="let product">{{ product.brand ?? '—' }}</td>
</ng-container>
```

Update the search label: `label="Buscar por código, nombre o marca"`.

### Detail: `product-detail/product-detail.html`

```html
<div class="detail-field">
  <dt>Marca</dt>
  <dd>{{ product.brand || '—' }}</dd>
</div>
```

### Spec fixture: `product-list/product-list.spec.ts`

Add `brand: null` to the `product` constant.

```bash
cd frontend
npx prettier --write src
npx ng test --watch=false
```

## 3. Manual check

1. Admin: create a product with a brand → list shows it → search by brand → edit keeps it → detail shows it.
2. Viewer: sees the column, no edit buttons.
3. Brand with 101 characters → form error. Through Swagger → 400.

## 4. Commit

```bash
git add -A
git commit -m "feat(products): add optional brand to products"
```

## Bigger variants

| Request | Recipes |
| --- | --- |
| Brand as its own table, picked from a list | [backend/new-resource.md](../../../backend/docs/playbook/new-resource.md) + [backend/relationships.md](../../../backend/docs/playbook/relationships.md) + [frontend/dialogs-and-selects.md](dialogs-and-selects.md#c-pick-a-related-record-which-control) |
| Filter the list by brand | [backend/lists-filters-sorting.md](../../../backend/docs/playbook/lists-filters-sorting.md) + [frontend/lists-filters.md](lists-filters.md) |
| Brand required | `[Required]` + `required string`, then a default for existing rows ([backend/migrations.md](../../../backend/docs/playbook/migrations.md#adding-a-required-column-to-a-table-with-data)) |
