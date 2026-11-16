import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { Product } from '../product.models';
import { ProductService } from '../product.service';
import { ProductSupplier } from '../product-supplier.models';
import { ProductSupplierService } from '../product-supplier.service';
import { ProductSupplierDialog } from '../product-supplier-dialog/product-supplier-dialog';

@Component({
  selector: 'app-product-detail',
  imports: [CurrencyPipe, MatButtonModule, MatTableModule, RouterLink],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss',
})
export class ProductDetail {
  private readonly products = inject(ProductService);
  private readonly productSuppliers = inject(ProductSupplierService);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);

  protected readonly id = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
  protected readonly isAdmin = inject(AuthService).isAdmin();
  protected readonly columns = this.isAdmin
    ? ['supplierName', 'lotNumber', 'price', 'stock', 'actions']
    : ['supplierName', 'lotNumber', 'price', 'stock'];
  protected readonly product = signal<Product | null>(null);
  protected readonly lots = signal<ProductSupplier[]>([]);
  protected readonly loadingLots = signal(true);

  constructor() {
    this.products
      .get(this.id)
      .pipe(
        catchError(() => {
          this.router.navigate(['/products']);
          return EMPTY;
        }),
      )
      .subscribe((product) => {
        this.product.set(product);
        this.loadLots();
      });
  }

  protected openDialog(productSupplier?: ProductSupplier): void {
    this.dialog
      .open(ProductSupplierDialog, {
        data: { productId: this.id, productSupplier },
        width: '480px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .subscribe((saved) => {
        if (saved) {
          this.notifications.success('Lote guardado');
          this.loadLots();
        }
      });
  }

  protected remove(lot: ProductSupplier): void {
    if (!confirm(`¿Eliminar el lote de "${lot.supplierName}"?`)) {
      return;
    }

    this.productSuppliers
      .remove(this.id, lot.id)
      .pipe(catchError(() => EMPTY))
      .subscribe(() => {
        this.notifications.success('Lote eliminado');
        this.loadLots();
      });
  }

  private loadLots(): void {
    this.loadingLots.set(true);
    this.productSuppliers
      .list(this.id)
      .pipe(
        catchError(() => EMPTY),
        finalize(() => this.loadingLots.set(false)),
      )
      .subscribe((lots) => this.lots.set(lots));
  }
}
