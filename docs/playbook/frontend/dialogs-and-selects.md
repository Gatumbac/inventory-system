# Dialogs and selects

## A. Edit in a dialog

**When:** small child records edited without leaving the page (like lots in the product detail).

Template: `features/products/product-supplier-dialog/` and how `product-detail.ts` opens it.

### The dialog component

```ts
export interface CategoryDialogData {
  category?: Category;
}

@Component({
  selector: 'app-category-dialog',
  imports: [ReactiveFormsModule, MatButtonModule, MatDialogModule, MatFormFieldModule, MatInputModule],
  templateUrl: './category-dialog.html',
})
export class CategoryDialog {
  private readonly service = inject(CategoryService);
  private readonly dialogRef = inject(MatDialogRef<CategoryDialog, boolean>);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly data = inject<CategoryDialogData>(MAT_DIALOG_DATA);

  protected readonly existing = this.data.category;
  protected readonly submitting = signal(false);
  protected readonly form = this.fb.group({
    name: [this.existing?.name ?? '', [Validators.required, Validators.maxLength(150)]],
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = this.form.getRawValue();
    const save$ = this.existing
      ? this.service.update(this.existing.id, request)
      : this.service.create(request);

    this.submitting.set(true);
    save$.subscribe({
      next: () => this.dialogRef.close(true),
      error: () => this.submitting.set(false),
    });
  }
}
```

Template (copy `product-supplier-dialog.html`):

```html
<h2 mat-dialog-title>{{ existing ? 'Editar categoría' : 'Nueva categoría' }}</h2>
<form [formGroup]="form" (ngSubmit)="submit()">
  <mat-dialog-content>
    <mat-form-field appearance="outline">
      <mat-label>Nombre</mat-label>
      <input matInput formControlName="name" />
    </mat-form-field>
  </mat-dialog-content>
  <mat-dialog-actions align="end">
    <button mat-button type="button" mat-dialog-close>Cancelar</button>
    <button mat-flat-button type="submit" [disabled]="submitting()">Guardar</button>
  </mat-dialog-actions>
</form>
```

### Opening it

```ts
private readonly dialog = inject(MatDialog);

protected openDialog(category?: Category): void {
  this.dialog
    .open(CategoryDialog, { data: { category }, width: '480px', maxWidth: '95vw' })
    .afterClosed()
    .subscribe((saved) => {
      if (saved) {
        this.notifications.success('Categoría guardada');
        this.load();
      }
    });
}
```

Row menu with a dialog instead of a link: `<app-row-actions [label]="c.name" (edit)="openDialog(c)" (remove)="remove(c)" />` (no `editLink`).

## B. Confirm before a destructive action

```ts
this.confirm
  .ask({
    title: '¿Eliminar categoría?',
    message: `Se eliminará "${category.name}".`,
    confirmLabel: 'Eliminar',
    destructive: true,
  })
  .pipe(
    filter(Boolean),
    switchMap(() => this.service.remove(category.id)),
    catchError(() => EMPTY),
  )
  .subscribe(() => {
    this.notifications.success('Categoría eliminada');
    this.load();
  });
```

`ConfirmService` is in `shared/confirm-dialog/confirm.service.ts`. It works for any yes/no question (`destructive: false` for a neutral one).

## C. Pick a related record: which control

| Size of the list | Control | Example |
| --- | --- | --- |
| Small and fixed (≤ 50: categories, statuses) | `mat-select` loaded once | below |
| Can grow (suppliers, products) | Autocomplete with server search | `features/suppliers/supplier-select` |

### mat-select for a FK

```ts
protected readonly categories = signal<Category[]>([]);

constructor() {
  this.categoryService
    .list(1, 50, '')
    .pipe(catchError(() => EMPTY))
    .subscribe((result) => this.categories.set(result.items));
}
```

```ts
categoryId: this.fb.control<number | null>(null),
```

```html
<mat-form-field appearance="outline">
  <mat-label>Categoría</mat-label>
  <mat-select formControlName="categoryId">
    <mat-option [value]="null">Sin categoría</mat-option>
    @for (c of categories(); track c.id) {
      <mat-option [value]="c.id">{{ c.name }}</mat-option>
    }
  </mat-select>
</mat-form-field>
```

`MatSelectModule` in `imports`. `patchValue({ categoryId: product.categoryId })` in edit mode.

### Autocomplete (reuse or copy SupplierSelect)

Use it as it is in any form:

```ts
supplier: new FormControl<Supplier | string>('', { nonNullable: true, validators: [supplierSelected] }),
```

```html
<app-supplier-select [control]="form.controls.supplier" />
```

The control holds the whole `Supplier` object. Send `(supplier as Supplier).id`.

For another entity, copy `supplier-select.ts`/`.html`/`.scss` and rename. It already has 300 ms debounce, server search, "Cargar más…" and the "must pick from the list" validator.
