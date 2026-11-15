import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { Product } from '../product.models';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-product-list',
  imports: [MatButtonModule, MatTableModule, RouterLink],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList {
  private readonly service = inject(ProductService);
  private readonly notifications = inject(NotificationService);

  protected readonly isAdmin = inject(AuthService).isAdmin();
  protected readonly columns = this.isAdmin
    ? ['code', 'name', 'description', 'attributes', 'actions']
    : ['code', 'name', 'description', 'attributes'];
  protected readonly products = signal<Product[]>([]);
  protected readonly loading = signal(true);

  constructor() {
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
        this.load();
      });
  }

  private load(): void {
    this.loading.set(true);
    this.service
      .list()
      .pipe(
        catchError(() => EMPTY),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((products) => this.products.set(products));
  }
}
