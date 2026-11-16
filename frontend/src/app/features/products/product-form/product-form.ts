import { Component, inject, signal } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError } from 'rxjs';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ProductRequest } from '../product.models';
import { ProductService } from '../product.service';

type AttributeGroup = FormGroup<{ name: FormControl<string>; value: FormControl<string> }>;

@Component({
  selector: 'app-product-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
  ],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
  host: { class: 'page page-narrow' },
})
export class ProductForm {
  private readonly service = inject(ProductService);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');

  protected readonly isEdit = this.id !== null;
  protected readonly submitting = signal(false);
  protected readonly loading = signal(this.id !== null);
  protected readonly form = this.fb.group({
    code: [
      '',
      [Validators.required, Validators.maxLength(50), Validators.pattern('^[A-Za-z0-9-]+$')],
    ],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.maxLength(1000)],
    attributes: new FormArray<AttributeGroup>([]),
  });
  protected readonly attributes = this.form.controls.attributes;

  constructor() {
    if (this.id !== null) {
      this.service
        .get(Number(this.id))
        .pipe(
          catchError(() => {
            this.router.navigate(['/products']);
            return EMPTY;
          }),
        )
        .subscribe((product) => {
          this.form.patchValue({
            code: product.code,
            name: product.name,
            description: product.description ?? '',
          });
          product.attributes.forEach((a) =>
            this.attributes.push(this.newAttribute(a.name, a.value)),
          );
          this.loading.set(false);
        });
    }
  }

  protected addAttribute(): void {
    this.attributes.push(this.newAttribute());
  }

  protected removeAttribute(index: number): void {
    this.attributes.removeAt(index);
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const request: ProductRequest = { ...value, description: value.description || null };
    const save$ =
      this.id === null
        ? this.service.create(request)
        : this.service.update(Number(this.id), request);

    this.submitting.set(true);
    save$.subscribe({
      next: () => {
        this.notifications.success('Producto guardado');
        this.router.navigate(['/products']);
      },
      error: () => this.submitting.set(false),
    });
  }

  private newAttribute(name = '', value = ''): AttributeGroup {
    return this.fb.group({
      name: [name, [Validators.required, Validators.maxLength(100)]],
      value: [value, [Validators.required, Validators.maxLength(200)]],
    });
  }
}
