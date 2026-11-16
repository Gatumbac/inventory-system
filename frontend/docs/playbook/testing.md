# Frontend tests

Vitest (through `ng test`) with jsdom. Spec files sit next to the code (`*.spec.ts`).

```bash
npx ng test --watch=false
npx ng test
```

The first runs once (like CI), the second watches.

Existing specs to copy:

| What | File |
| --- | --- |
| HTTP service | `features/products/product-supplier.service.spec.ts` |
| Component with mocked services | `features/products/product-list/product-list.spec.ts` |
| Guard | `core/auth/admin.guard.spec.ts`, `auth.guard.spec.ts` |
| Shared component with inputs/outputs | `shared/search-field/search-field.spec.ts`, `shared/skeleton-rows/skeleton-rows.spec.ts` |
| Service around a dialog | `shared/confirm-dialog/confirm.service.spec.ts` |
| Autocomplete | `features/suppliers/supplier-select/supplier-select.spec.ts` |

## Service spec (HttpTestingController)

```ts
describe('CategoryService', () => {
  const base = `${environment.apiUrl}/categories`;
  let service: CategoryService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CategoryService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('sends the filter as a query param', () => {
    service.list(1, 10, '', 3).subscribe();
    const req = http.expectOne(
      (r) => r.url === base && r.params.get('supplierId') === '3',
    );
    expect(req.request.method).toBe('GET');
    req.flush({ items: [], page: 1, pageSize: 10, totalCount: 0, totalPages: 0 });
  });
});
```

Build URLs from `environment.apiUrl`. In tests it is the full `http://localhost:5078/api`.

A string passed to `expectOne` is compared with the full URL, query string included. When the request has params, use the function form above.

## Component spec

The pattern from `product-list.spec.ts`:

```ts
const setup = () => {
  localStorage.setItem('role', 'Admin');
  TestBed.configureTestingModule({
    imports: [CategoryList],
    providers: [
      provideRouter([]),
      { provide: CategoryService, useValue: { list: () => of(page), remove: vi.fn() } },
    ],
  });
  const fixture = TestBed.createComponent(CategoryList);
  fixture.detectChanges();
  return { fixture, element: fixture.nativeElement as HTMLElement };
};

afterEach(() => localStorage.removeItem('role'));

it('shows the new column', () => {
  const { element } = setup();
  expect(element.textContent).toContain('Teléfono');
});
```

- Replace services with `{ provide: X, useValue: {...} }`, returning `of(...)` for Observables.
- `vi.fn()` and `vi.spyOn(...)` are Vitest's `jest.fn()`/`jest.spyOn()`. They are global, no import needed.
- The role comes from `localStorage`, so set it before creating the component.

## When the build of tests fails after a model change

Specs that build full objects (`const product: Product = {...}`) need the new required properties. Make the new property `| null` and add it to the fixtures.
