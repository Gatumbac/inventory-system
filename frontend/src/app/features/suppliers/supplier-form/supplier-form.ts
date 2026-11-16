import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError } from 'rxjs';
import { NotificationService } from '../../../core/notifications/notification.service';
import { SupplierRequest } from '../supplier.models';
import { SupplierService } from '../supplier.service';

@Component({
  selector: 'app-supplier-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
  ],
  templateUrl: './supplier-form.html',
  host: { class: 'page page-narrow' },
})
export class SupplierForm {
  private readonly service = inject(SupplierService);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');

  protected readonly isEdit = this.id !== null;
  protected readonly submitting = signal(false);
  protected readonly loading = signal(this.id !== null);
  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    description: ['', Validators.maxLength(500)],
  });

  constructor() {
    if (this.id !== null) {
      this.service
        .get(Number(this.id))
        .pipe(
          catchError(() => {
            this.router.navigate(['/suppliers']);
            return EMPTY;
          }),
        )
        .subscribe((supplier) => {
          this.form.patchValue({
            name: supplier.name,
            description: supplier.description ?? '',
          });
          this.loading.set(false);
        });
    }
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const request: SupplierRequest = { ...value, description: value.description || null };
    const save$ =
      this.id === null
        ? this.service.create(request)
        : this.service.update(Number(this.id), request);

    this.submitting.set(true);
    save$.subscribe({
      next: () => {
        this.notifications.success('Proveedor guardado');
        this.router.navigate(['/suppliers']);
      },
      error: () => this.submitting.set(false),
    });
  }
}
