import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PagedResult, Product, ProductRequest } from './product.models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/products`;

  list(page: number, pageSize: number, search: string) {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResult<Product>>(this.url, { params });
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

  remove(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
