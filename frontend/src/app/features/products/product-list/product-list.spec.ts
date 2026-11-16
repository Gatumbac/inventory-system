import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { PagedResult } from '../../../core/http/paged-result';
import { Product } from '../product.models';
import { ProductService } from '../product.service';
import { ProductList } from './product-list';

const product: Product = {
  id: 7,
  code: 'LAP-001',
  name: 'Laptop',
  description: null,
  attributes: [],
};

const page: PagedResult<Product> = {
  items: [product],
  page: 1,
  pageSize: 10,
  totalCount: 1,
  totalPages: 1,
};

describe('ProductList', () => {
  const setup = () => {
    localStorage.setItem('role', 'Admin');
    TestBed.configureTestingModule({
      imports: [ProductList],
      providers: [
        provideRouter([]),
        { provide: ProductService, useValue: { list: () => of(page) } },
      ],
    });
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
    const fixture = TestBed.createComponent(ProductList);
    fixture.detectChanges();
    return { navigate, element: fixture.nativeElement as HTMLElement };
  };

  afterEach(() => localStorage.removeItem('role'));

  it('opens the product detail when a row is clicked', () => {
    const { navigate, element } = setup();

    element.querySelector<HTMLElement>('tr.row-link td')?.click();

    expect(navigate).toHaveBeenCalledWith(['/products', 7]);
  });

  it('does not navigate when the click comes from a button inside the row', () => {
    const { navigate, element } = setup();

    const kebab = element.querySelector<HTMLElement>('tr.row-link app-row-actions button');
    expect(kebab).not.toBeNull();
    kebab?.click();

    expect(navigate).not.toHaveBeenCalled();
  });
});
