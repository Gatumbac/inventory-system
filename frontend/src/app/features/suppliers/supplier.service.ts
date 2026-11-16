import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../../core/http/paged-result';
import { Supplier, SupplierRequest } from './supplier.models';

@Injectable({ providedIn: 'root' })
export class SupplierService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/suppliers`;

  list(page: number, pageSize: number, search: string) {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResult<Supplier>>(this.url, { params });
  }

  get(id: number) {
    return this.http.get<Supplier>(`${this.url}/${id}`);
  }

  create(request: SupplierRequest) {
    return this.http.post<Supplier>(this.url, request);
  }

  update(id: number, request: SupplierRequest) {
    return this.http.put<Supplier>(`${this.url}/${id}`, request);
  }

  remove(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
