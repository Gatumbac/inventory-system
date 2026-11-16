import { Component, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorIntl, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { Router, RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { SpanishPaginatorIntl } from '../../../core/layout/paginator-intl';
import { NotificationService } from '../../../core/notifications/notification.service';
import { RowActions } from '../../../shared/row-actions/row-actions';
import { SearchField } from '../../../shared/search-field/search-field';
import { SkeletonRows } from '../../../shared/skeleton-rows/skeleton-rows';
import { Product } from '../product.models';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-product-list',
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
  templateUrl: './product-list.html',
  host: { class: 'page' },
  providers: [{ provide: MatPaginatorIntl, useClass: SpanishPaginatorIntl }],
})
export class ProductList {
  private readonly service = inject(ProductService);
  private readonly notifications = inject(NotificationService);
  private readonly router = inject(Router);

  protected readonly searchField = viewChild.required(SearchField);

  protected readonly isAdmin = inject(AuthService).isAdmin();
  protected readonly columns = this.isAdmin
    ? ['code', 'name', 'description', 'attributes', 'actions', 'chevron']
    : ['code', 'name', 'description', 'attributes', 'chevron'];
  protected readonly pageSizeOptions = [5, 10, 25, 50];
  protected readonly products = signal<Product[]>([]);
  protected readonly loading = signal(true);
  protected readonly failed = signal(false);
  protected readonly page = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly search = signal('');
  protected readonly totalCount = signal(0);

  constructor() {
    this.load();
  }

  protected open(event: MouseEvent, product: Product): void {
    if ((event.target as HTMLElement).closest('a, button')) {
      return;
    }

    this.router.navigate(['/products', product.id]);
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

  protected remove(product: Product): void {
    if (!confirm(`¿Eliminar "${product.name}"?`)) {
      return;
    }

    this.service
      .remove(product.id)
      .pipe(catchError(() => EMPTY))
      .subscribe(() => {
        this.notifications.success('Producto eliminado');
        if (this.products().length === 1 && this.page() > 1) {
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
        this.products.set(result.items);
        this.totalCount.set(result.totalCount);
      });
  }
}
