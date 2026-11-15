import { Component, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { EMPTY, catchError, finalize } from 'rxjs';
import { Product } from '../product.models';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-product-list',
  imports: [MatTableModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList {
  protected readonly columns = ['code', 'name', 'description', 'attributes'];
  protected readonly products = signal<Product[]>([]);
  protected readonly loading = signal(true);

  constructor() {
    inject(ProductService)
      .list()
      .pipe(
        catchError(() => EMPTY),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((products) => this.products.set(products));
  }
}
