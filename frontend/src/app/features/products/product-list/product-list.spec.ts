import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { PagedResult } from '../../../core/http/paged-result';
import { ConfirmService } from '../../../shared/confirm-dialog/confirm.service';
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
  const setup = (confirmed = true) => {
    localStorage.setItem('role', 'Admin');
    const remove = vi.fn().mockReturnValue(of(undefined));
    TestBed.configureTestingModule({
      imports: [ProductList],
      providers: [
        provideRouter([]),
        { provide: ProductService, useValue: { list: () => of(page), remove } },
        { provide: ConfirmService, useValue: { ask: () => of(confirmed) } },
      ],
    });
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
    const fixture = TestBed.createComponent(ProductList);
    fixture.detectChanges();
    return { navigate, remove, fixture, element: fixture.nativeElement as HTMLElement };
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

  it('removes the product after the user confirms', () => {
    const { remove, fixture } = setup(true);

    (fixture.componentInstance as unknown as { remove(p: Product): void }).remove(product);

    expect(remove).toHaveBeenCalledWith(7);
  });

  it('keeps the product when the user cancels', () => {
    const { remove, fixture } = setup(false);

    (fixture.componentInstance as unknown as { remove(p: Product): void }).remove(product);

    expect(remove).not.toHaveBeenCalled();
  });
});
