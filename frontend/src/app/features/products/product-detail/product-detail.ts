import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EMPTY, catchError, filter, finalize, switchMap } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ConfirmService } from '../../../shared/confirm-dialog/confirm.service';
import { RowActions } from '../../../shared/row-actions/row-actions';
import { SkeletonRows } from '../../../shared/skeleton-rows/skeleton-rows';
import { Product } from '../product.models';
import { ProductService } from '../product.service';
import { ProductSupplier } from '../product-supplier.models';
import { ProductSupplierService } from '../product-supplier.service';
import { ProductSupplierDialog } from '../product-supplier-dialog/product-supplier-dialog';

@Component({
  selector: 'app-product-detail',
  imports: [
    CurrencyPipe,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTableModule,
    RouterLink,
    RowActions,
    SkeletonRows,
  ],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss',
  host: { class: 'page' },
})
export class ProductDetail {
  private readonly products = inject(ProductService);
  private readonly productSuppliers = inject(ProductSupplierService);
  private readonly dialog = inject(MatDialog);
  private readonly confirm = inject(ConfirmService);
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
    this.confirm
      .ask({
        title: '¿Eliminar lote?',
        message: `Se eliminará el lote de "${lot.supplierName}".`,
        confirmLabel: 'Eliminar',
        destructive: true,
      })
      .pipe(
        filter(Boolean),
        switchMap(() => this.productSuppliers.remove(this.id, lot.id)),
        catchError(() => EMPTY),
      )
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
