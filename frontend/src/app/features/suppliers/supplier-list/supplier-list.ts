import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { Supplier } from '../supplier.models';
import { SupplierService } from '../supplier.service';

@Component({
  selector: 'app-supplier-list',
  imports: [
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatPaginatorModule,
    MatTableModule,
    RouterLink,
  ],
  templateUrl: './supplier-list.html',
  styleUrl: './supplier-list.scss',
})
export class SupplierList {
  private readonly service = inject(SupplierService);
  private readonly notifications = inject(NotificationService);

  protected readonly isAdmin = inject(AuthService).isAdmin();
  protected readonly columns = this.isAdmin
    ? ['name', 'description', 'actions']
    : ['name', 'description'];
  protected readonly pageSizeOptions = [5, 10, 25, 50];
  protected readonly suppliers = signal<Supplier[]>([]);
  protected readonly loading = signal(true);
  protected readonly page = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly search = signal('');
  protected readonly totalCount = signal(0);

  constructor() {
    this.load();
  }

  protected onPage(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.load();
  }

  protected onSearch(value: string): void {
    this.search.set(value.trim());
    this.page.set(1);
    this.load();
  }

  protected remove(supplier: Supplier): void {
    if (!confirm(`¿Eliminar "${supplier.name}"?`)) {
      return;
    }

    this.service
      .remove(supplier.id)
      .pipe(catchError(() => EMPTY))
      .subscribe(() => {
        this.notifications.success('Proveedor eliminado');
        if (this.suppliers().length === 1 && this.page() > 1) {
          this.page.update((page) => page - 1);
        }
        this.load();
      });
  }

  private load(): void {
    this.loading.set(true);
    this.service
      .list(this.page(), this.pageSize(), this.search())
      .pipe(
        catchError(() => EMPTY),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((result) => {
        this.suppliers.set(result.items);
        this.totalCount.set(result.totalCount);
      });
  }
}
