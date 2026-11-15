import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Product, ProductRequest } from './product.models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/products`;

  list() {
    return this.http.get<Product[]>(this.url);
  }

  get(id: number) {
    return this.http.get<Product>(`${this.url}/${id}`);
  }

  create(request: ProductRequest) {
    return this.http.post<Product>(this.url, request);
  }

  update(id: number, request: ProductRequest) {
    return this.http.put<Product>(`${this.url}/${id}`, request);
  }
}
