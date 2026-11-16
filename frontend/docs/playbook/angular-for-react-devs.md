# Angular for React developers

Angular comes with the router, the HTTP client, forms, dependency injection and a component library (Material). In React each of those is a library you pick. This app uses modern Angular: standalone components and signals, which feel close to function components with hooks.

## Mapping

| Angular in this repo | React equivalent |
| --- | --- |
| `@Component({...}) export class SupplierList` | `function SupplierList()` |
| `templateUrl: './supplier-list.html'` | the JSX returned by the component, in its own file |
| `imports: [MatTableModule, RouterLink, SearchField]` | the `import` lines at the top: a template can only use what is listed here |
| `signal(0)` | `useState(0)` |
| read `count()` | read `count` |
| `count.set(1)` / `count.update(c => c + 1)` | `setCount(1)` / `setCount(c => c + 1)` |
| `computed(() => a() + b())` | `useMemo(() => a + b, [a, b])`, but dependencies are tracked automatically |
| `input<string>()` / `input.required<string>()` | a prop |
| `output<string>()` + `this.search.emit(x)` | a callback prop `onSearch(x)` |
| `constructor() { this.load(); }` | `useEffect(() => load(), [])` |
| `@Injectable({ providedIn: 'root' }) class SupplierService` | a module with API functions, or a context provider at the root: one shared instance |
| `inject(SupplierService)` | `useContext(...)` / importing the module |
| `app.routes.ts` | `createBrowserRouter([...])` |
| `loadComponent: () => import(...)` | `React.lazy(() => import(...))` |
| `canActivate: [authGuard]` | a `<ProtectedRoute>` that renders `<Navigate to="/login" />` |
| `<router-outlet />` | `<Outlet />` |
| `routerLink="/suppliers"` | `<Link to="/suppliers">` |
| `inject(ActivatedRoute).snapshot.paramMap.get('id')` | `useParams().id` |
| `inject(Router).navigate(['/products', id])` | `navigate(`/products/${id}`)` |
| `HttpClient` + interceptors | axios + `axios.interceptors` |
| Reactive forms (`fb.group`, `Validators`) | react-hook-form + a schema |
| `FormArray` | `useFieldArray` |
| Angular Material | MUI |

The big difference: a React function runs again on every render, while an Angular class is created **once**. Its methods run when called, and the template re-reads signals when they change. So there are no dependency arrays, no `useCallback`, and no stale closures.

## Template syntax

| Angular | JSX |
| --- | --- |
| `{{ supplier.name }}` | `{supplier.name}` |
| `[dataSource]="suppliers()"` | `dataSource={suppliers}` |
| `(click)="remove(supplier)"` | `onClick={() => remove(supplier)}` |
| `(search)="onSearch($event)"` | `onSearch={onSearch}`; `$event` is what the child emitted |
| `[class.open]="navOpen()"` | `className={navOpen ? 'open' : ''}` |
| `[attr.title]="x"` | `title={x}` |
| `@if (isAdmin) { … } @else { … }` | `{isAdmin ? … : …}` |
| `@for (s of suppliers(); track s.id) { … }` | `{suppliers.map(s => <… key={s.id} />)}` |
| `@if (product(); as product) { … }` | `{product && (() => …)()}`: the alias is non-null inside |
| `#input` then `input.value` | `useRef` |
| `{{ price \| currency }}` | `formatCurrency(price)` (pipes are formatting functions) |

## Classes and visibility

```ts
export class SupplierList {
  private readonly service = inject(SupplierService);
  protected readonly suppliers = signal<Supplier[]>([]);
  protected remove(supplier: Supplier): void {}
  private load(): void {}
}
```

- `private`: used only inside the class.
- `protected`: used by the template too.
- `readonly`: the field is never reassigned (the signal inside still changes).

## RxJS: the part React does not have

`HttpClient` returns **Observables**: think of a Promise that can emit several times and is lazy.

| Operator | Meaning |
| --- | --- |
| `.subscribe(fn)` | `.then(fn)`. Without `subscribe` the request is **never sent**. |
| `catchError(() => EMPTY)` | `.catch(() => {})`, ignore (the interceptor already showed the error) |
| `finalize(fn)` | `.finally(fn)` |
| `filter(Boolean)` | continue only if true (the user confirmed) |
| `switchMap(() => other$)` | then call the next request; a new value cancels the previous one |
| `debounceTime(300)` | debounce |
| `map(x => …)` | transform the value |
| `takeUntilDestroyed()` | the cleanup in `useEffect`'s return: unsubscribe when the component dies |

Example from `supplier-list.ts`:

```ts
this.confirm
  .ask({ title: '¿Eliminar proveedor?', message: '…', confirmLabel: 'Eliminar', destructive: true })
  .pipe(
    filter(Boolean),
    switchMap(() => this.service.remove(supplier.id)),
    catchError(() => EMPTY),
  )
  .subscribe(() => {
    this.notifications.success('Proveedor eliminado');
    this.load();
  });
```

The same in React with async/await:

```ts
if (!(await confirm({...}))) return;
try { await api.remove(id); toast('Proveedor eliminado'); load(); } catch {}
```

## A whole list page in React terms

```tsx
function SupplierList() {
  const [suppliers, setSuppliers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  useEffect(() => { load(); }, []);
  function load() {
    setLoading(true);
    api.list(page, pageSize, search).then(r => { setSuppliers(r.items); setTotal(r.totalCount); })
      .finally(() => setLoading(false));
  }
}
```

That is `supplier-list.ts` almost line by line.
