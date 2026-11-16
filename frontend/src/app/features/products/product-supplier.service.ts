import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  CreateProductSupplierRequest,
  ProductSupplier,
  ProductSupplierRequest,
} from './product-supplier.models';

@Injectable({ providedIn: 'root' })
export class ProductSupplierService {
  private readonly http = inject(HttpClient);

  list(productId: number) {
    return this.http.get<ProductSupplier[]>(this.url(productId));
  }

  create(productId: number, request: CreateProductSupplierRequest) {
    return this.http.post<ProductSupplier>(this.url(productId), request);
  }

  update(productId: number, id: number, request: ProductSupplierRequest) {
    return this.http.put<ProductSupplier>(`${this.url(productId)}/${id}`, request);
  }

  remove(productId: number, id: number) {
    return this.http.delete<void>(`${this.url(productId)}/${id}`);
  }

  private url(productId: number): string {
    return `${environment.apiUrl}/products/${productId}/suppliers`;
  }
}
