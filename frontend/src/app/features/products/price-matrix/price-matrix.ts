import { CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorIntl, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { EMPTY, catchError, finalize } from 'rxjs';
import { SpanishPaginatorIntl } from '../../../core/layout/paginator-intl';
import { SearchField } from '../../../shared/search-field/search-field';
import { SkeletonRows } from '../../../shared/skeleton-rows/skeleton-rows';
import { PriceMatrixRow, PriceMatrixSupplier } from '../product.models';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-price-matrix',
  imports: [
    CurrencyPipe,
    MatButtonModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatTableModule,
    RouterLink,
    SearchField,
    SkeletonRows,
  ],
  templateUrl: './price-matrix.html',
  styleUrl: './price-matrix.scss',
  host: { class: 'page' },
  providers: [{ provide: MatPaginatorIntl, useClass: SpanishPaginatorIntl }],
})
export class PriceMatrix {
  private readonly service = inject(ProductService);

  protected readonly searchField = viewChild.required(SearchField);

  protected readonly pageSizeOptions = [5, 10, 25, 50];
  protected readonly suppliers = signal<PriceMatrixSupplier[]>([]);
  protected readonly rows = signal<PriceMatrixRow[]>([]);
  protected readonly columns = computed(() => [
    'product',
    ...this.suppliers().map((supplier) => this.supplierColumn(supplier)),
  ]);
  protected readonly loading = signal(true);
  protected readonly failed = signal(false);
  protected readonly page = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly search = signal('');
  protected readonly totalCount = signal(0);

  constructor() {
    this.load();
  }

  protected supplierColumn(supplier: PriceMatrixSupplier): string {
    return `supplier-${supplier.id}`;
  }

  protected priceOf(row: PriceMatrixRow, supplier: PriceMatrixSupplier): number | null {
    return row.prices.find((price) => price.supplierId === supplier.id)?.price ?? null;
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

  private load(): void {
    this.loading.set(true);
    this.failed.set(false);
    this.service
      .priceMatrix(this.page(), this.pageSize(), this.search())
      .pipe(
        catchError(() => {
          this.failed.set(true);
          return EMPTY;
        }),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((matrix) => {
        this.suppliers.set(matrix.suppliers);
        this.rows.set(matrix.products.items);
        this.totalCount.set(matrix.products.totalCount);
      });
  }
}
