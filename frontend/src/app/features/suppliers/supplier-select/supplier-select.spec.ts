import { FormControl } from '@angular/forms';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { PagedResult } from '../../../core/http/paged-result';
import { Supplier } from '../supplier.models';
import { SupplierService } from '../supplier.service';
import { SupplierSelect, supplierSelected } from './supplier-select';

const supplier = (id: number): Supplier => ({ id, name: `Supplier ${id}`, description: null });

const pageOf = (items: Supplier[], page: number, totalPages: number): PagedResult<Supplier> => ({
  items,
  page,
  pageSize: 10,
  totalCount: items.length,
  totalPages,
});

describe('SupplierSelect', () => {
  const list = vi.fn();

  const setup = () => {
    TestBed.configureTestingModule({
      imports: [SupplierSelect],
      providers: [{ provide: SupplierService, useValue: { list } }],
    });
    const fixture = TestBed.createComponent(SupplierSelect);
    fixture.componentRef.setInput('control', new FormControl<Supplier | string>(''));
    fixture.detectChanges();
    return fixture.componentInstance as unknown as {
      onFocus(): void;
      onInput(value: string): void;
      loadMore(): void;
      options(): Supplier[];
      hasMore(): boolean;
    };
  };

  beforeEach(() => {
    list.mockReset();
    vi.useFakeTimers();
  });

  afterEach(() => vi.useRealTimers());

  it('loads the first page on focus', () => {
    list.mockReturnValue(of(pageOf([supplier(1)], 1, 1)));
    const select = setup();

    select.onFocus();

    expect(list).toHaveBeenCalledWith(1, 10, '');
    expect(select.options()).toEqual([supplier(1)]);
    expect(select.hasMore()).toBe(false);
  });

  it('debounces typing and searches with the trimmed term', () => {
    list.mockReturnValue(of(pageOf([supplier(2)], 1, 1)));
    const select = setup();

    select.onInput(' ac');
    select.onInput(' acm ');
    expect(list).not.toHaveBeenCalled();
    vi.advanceTimersByTime(300);

    expect(list).toHaveBeenCalledTimes(1);
    expect(list).toHaveBeenCalledWith(1, 10, 'acm');
  });

  it('appends the next page when loading more', () => {
    list
      .mockReturnValueOnce(of(pageOf([supplier(1)], 1, 2)))
      .mockReturnValueOnce(of(pageOf([supplier(2)], 2, 2)));
    const select = setup();

    select.onFocus();
    expect(select.hasMore()).toBe(true);
    select.loadMore();

    expect(list).toHaveBeenLastCalledWith(2, 10, '');
    expect(select.options()).toEqual([supplier(1), supplier(2)]);
    expect(select.hasMore()).toBe(false);
  });
});

describe('supplierSelected', () => {
  it('rejects free text and empty values', () => {
    expect(supplierSelected(new FormControl(''))).toEqual({ supplierRequired: true });
    expect(supplierSelected(new FormControl('acme'))).toEqual({ supplierRequired: true });
  });

  it('accepts a selected supplier', () => {
    expect(supplierSelected(new FormControl(supplier(1)))).toBeNull();
  });
});
