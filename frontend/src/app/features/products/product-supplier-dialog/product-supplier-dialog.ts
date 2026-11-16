import { Component, inject, signal } from '@angular/core';
import {
  FormControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Supplier } from '../../suppliers/supplier.models';
import { SupplierSelect, supplierSelected } from '../../suppliers/supplier-select/supplier-select';
import { ProductSupplier } from '../product-supplier.models';
import { ProductSupplierService } from '../product-supplier.service';

export interface ProductSupplierDialogData {
  productId: number;
  productSupplier?: ProductSupplier;
}

const MAX_STOCK = 2147483647;

@Component({
  selector: 'app-product-supplier-dialog',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    SupplierSelect,
  ],
  templateUrl: './product-supplier-dialog.html',
  styleUrl: './product-supplier-dialog.scss',
})
export class ProductSupplierDialog {
  private readonly service = inject(ProductSupplierService);
  private readonly dialogRef = inject(MatDialogRef<ProductSupplierDialog, boolean>);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly data = inject<ProductSupplierDialogData>(MAT_DIALOG_DATA);

  protected readonly existing = this.data.productSupplier;
  protected readonly submitting = signal(false);
  protected readonly form = this.fb.group({
    supplier: new FormControl<Supplier | string>('', {
      nonNullable: true,
      validators: [supplierSelected],
    }),
    lotNumber: [this.existing?.lotNumber ?? '', Validators.maxLength(50)],
    price: [this.existing?.price ?? 0, [Validators.required, Validators.min(0)]],
    stock: [
      this.existing?.stock ?? 0,
      [
        Validators.required,
        Validators.min(0),
        Validators.max(MAX_STOCK),
        Validators.pattern('^[0-9]+$'),
      ],
    ],
  });

  constructor() {
    if (this.existing) {
      this.form.controls.supplier.disable();
    }
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { supplier, lotNumber, price, stock } = this.form.getRawValue();
    const body = { lotNumber: lotNumber.trim() || null, price, stock };
    const save$ = this.existing
      ? this.service.update(this.data.productId, this.existing.id, body)
      : this.service.create(this.data.productId, {
          ...body,
          supplierId: (supplier as Supplier).id,
        });

    this.submitting.set(true);
    save$.subscribe({
      next: () => this.dialogRef.close(true),
      error: () => this.submitting.set(false),
    });
  }
}
