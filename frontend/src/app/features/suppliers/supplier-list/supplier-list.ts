import { Component, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorIntl, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { EMPTY, catchError, filter, finalize, switchMap } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { SpanishPaginatorIntl } from '../../../core/layout/paginator-intl';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ConfirmService } from '../../../shared/confirm-dialog/confirm.service';
import { RowActions } from '../../../shared/row-actions/row-actions';
import { SearchField } from '../../../shared/search-field/search-field';
import { SkeletonRows } from '../../../shared/skeleton-rows/skeleton-rows';
import { Supplier } from '../supplier.models';
import { SupplierService } from '../supplier.service';

@Component({
  selector: 'app-supplier-list',
  imports: [
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatTableModule,
    RouterLink,
    RowActions,
    SearchField,
    SkeletonRows,
  ],
  templateUrl: './supplier-list.html',
  host: { class: 'page' },
  providers: [{ provide: MatPaginatorIntl, useClass: SpanishPaginatorIntl }],
})
export class SupplierList {
  private readonly service = inject(SupplierService);
  private readonly notifications = inject(NotificationService);
  private readonly confirm = inject(ConfirmService);

  protected readonly searchField = viewChild.required(SearchField);

  protected readonly isAdmin = inject(AuthService).isAdmin();
  protected readonly columns = this.isAdmin
    ? ['name', 'description', 'actions']
    : ['name', 'description'];
  protected readonly pageSizeOptions = [5, 10, 25, 50];
  protected readonly suppliers = signal<Supplier[]>([]);
  protected readonly loading = signal(true);
  protected readonly failed = signal(false);
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
    this.confirm
      .ask({
        title: '¿Eliminar proveedor?',
        message: `Se eliminará "${supplier.name}".`,
        confirmLabel: 'Eliminar',
        destructive: true,
      })
      .pipe(
        filter(Boolean),
        switchMap(() => this.service.remove(supplier.id)),
        catchError(() => EMPTY),
      )
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
    this.failed.set(false);
    this.service
      .list(this.page(), this.pageSize(), this.search())
      .pipe(
        catchError(() => {
          this.failed.set(true);
          return EMPTY;
        }),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((result) => {
        this.suppliers.set(result.items);
        this.totalCount.set(result.totalCount);
      });
  }
}
