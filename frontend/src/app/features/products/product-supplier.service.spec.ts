import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { ProductSupplierService } from './product-supplier.service';

describe('ProductSupplierService', () => {
  const base = `${environment.apiUrl}/products/7/suppliers`;
  let service: ProductSupplierService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ProductSupplierService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('lists the suppliers of a product', () => {
    service.list(7).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('creates a lot with the supplier id', () => {
    const body = { supplierId: 3, lotNumber: 'L-1', price: 12.5, stock: 4 };
    service.create(7, body).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({});
  });

  it('updates a lot without the supplier id', () => {
    const body = { lotNumber: null, price: 9, stock: 0 };
    service.update(7, 11, body).subscribe();
    const req = http.expectOne(`${base}/11`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(body);
    req.flush({});
  });

  it('removes a lot', () => {
    service.remove(7, 11).subscribe();
    const req = http.expectOne(`${base}/11`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
