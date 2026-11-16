# Forms and validation

Reactive forms, typed, with `NonNullableFormBuilder` (`supplier-form.ts`, `product-form.ts`, `product-supplier-dialog.ts`).

## The shape of every form component

```ts
private readonly fb = inject(NonNullableFormBuilder);
private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');

protected readonly isEdit = this.id !== null;
protected readonly submitting = signal(false);
protected readonly loading = signal(this.id !== null);
protected readonly form = this.fb.group({
  name: ['', [Validators.required, Validators.maxLength(150)]],
});
```

1. Edit mode: `service.get(id)` → `form.patchValue(...)` → `loading.set(false)`. On error, navigate back to the list.
2. `submit()`: if `form.invalid` → `markAllAsTouched()` (shows every error) → return.
3. `getRawValue()` → build the Request (empty strings → `null`).
4. `create` or `update` → on success, a snackbar and navigation. On error, `submitting.set(false)` (the interceptor already showed the message).

Template:

```html
<form class="form-column" [formGroup]="form" (ngSubmit)="submit()">
  <mat-form-field appearance="outline">
    <mat-label>Nombre</mat-label>
    <input matInput formControlName="name" />
    @if (form.controls.name.hasError('required')) {
      <mat-error>El nombre es obligatorio</mat-error>
    } @else if (form.controls.name.hasError('maxlength')) {
      <mat-error>Máximo 150 caracteres</mat-error>
    }
  </mat-form-field>
  <div class="form-actions">
    <a mat-button routerLink="/suppliers">Cancelar</a>
    <button mat-flat-button type="submit" [disabled]="submitting()">Guardar</button>
  </div>
</form>
```

## Built-in validators

| Validator | Error key for `hasError` |
| --- | --- |
| `Validators.required` | `required` |
| `Validators.maxLength(150)` / `minLength(3)` | `maxlength` / `minlength` |
| `Validators.min(0)` / `max(100)` | `min` / `max` |
| `Validators.pattern('^[A-Za-z0-9-]+$')` | `pattern` |
| `Validators.email` | `email` |

Keep the limits equal to the backend DTO. The backend validates again anyway, and returns 400 with the message shown by the interceptor.

## Field types

```html
<input matInput type="number" formControlName="price" min="0" step="0.01" />
<textarea matInput rows="3" formControlName="description"></textarea>
<mat-checkbox formControlName="active">Activo</mat-checkbox>
<mat-select formControlName="categoryId">
  @for (c of categories(); track c.id) {
    <mat-option [value]="c.id">{{ c.name }}</mat-option>
  }
</mat-select>
```

Add the module of each one to the component `imports`: `MatCheckboxModule`, `MatSelectModule` (`MatFormFieldModule` and `MatInputModule` are already there).

Dates: `MatDatepickerModule` + `provideNativeDateAdapter()` in the component `providers`. The control holds a `Date`. Send it with `.toISOString()`.

## Custom validator

Same as `supplierSelected` in `supplier-select.ts`:

```ts
export function notOnlySpaces(control: AbstractControl): ValidationErrors | null {
  return typeof control.value === 'string' && control.value.trim() === ''
    ? { onlySpaces: true }
    : null;
}
```

```ts
name: ['', [Validators.required, notOnlySpaces]],
```

```html
@else if (form.controls.name.hasError('onlySpaces')) {
  <mat-error>No puede estar vacío</mat-error>
}
```

## Rule across two fields

**When:** "max stock must be greater than min stock".

```ts
function minBelowMax(group: AbstractControl): ValidationErrors | null {
  const min = group.get('minStock')?.value;
  const max = group.get('maxStock')?.value;
  return min !== null && max !== null && min > max ? { minAboveMax: true } : null;
}

protected readonly form = this.fb.group(
  {
    minStock: [0, Validators.min(0)],
    maxStock: [0, Validators.min(0)],
  },
  { validators: minBelowMax },
);
```

```html
@if (form.hasError('minAboveMax') && form.touched) {
  <mat-error>El mínimo no puede ser mayor que el máximo</mat-error>
}
```

## Dynamic list of fields (FormArray)

See `product-form.ts` for attributes: `attributes: new FormArray<AttributeGroup>([])`, `addAttribute()`, `removeAttribute(i)`, a helper `newAttribute(name, value)`, and in edit mode `product.attributes.forEach(a => this.attributes.push(this.newAttribute(a.name, a.value)))`.

## Disable a field

In edit mode, the lot dialog disables the supplier: `this.form.controls.supplier.disable()`. `getRawValue()` still includes disabled controls, while `value` does not.

## Verify

- Submit empty → every required field shows its error.
- Break each rule once.
- A server-side error (duplicate name) → snackbar with the API message, and the button is enabled again.
