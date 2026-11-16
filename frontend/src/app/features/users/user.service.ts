import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../../core/http/paged-result';
import { User, UserRequest } from './user.models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/users`;

  list(page: number, pageSize: number, search: string) {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<PagedResult<User>>(this.url, { params });
  }

  create(request: UserRequest) {
    return this.http.post<User>(this.url, request);
  }

  remove(id: number) {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
