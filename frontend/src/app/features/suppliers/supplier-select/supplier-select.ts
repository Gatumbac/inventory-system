import { Component, inject, input, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormControl,
  ReactiveFormsModule,
  ValidationErrors,
} from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { EMPTY, Subject, catchError, debounceTime, map, switchMap } from 'rxjs';
import { Supplier } from '../supplier.models';
import { SupplierService } from '../supplier.service';

const PAGE_SIZE = 10;
const SEARCH_DEBOUNCE_MS = 300;

export function supplierSelected(control: AbstractControl): ValidationErrors | null {
  return typeof control.value === 'object' && control.value !== null
    ? null
    : { supplierRequired: true };
}

@Component({
  selector: 'app-supplier-select',
  imports: [
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './supplier-select.html',
  styleUrl: './supplier-select.scss',
})
export class SupplierSelect {
  private readonly service = inject(SupplierService);
  private readonly requests = new Subject<{ term: string; page: number }>();
  private readonly typing = new Subject<string>();
  private term = '';
  private page = 1;

  readonly control = input.required<FormControl<Supplier | string>>();

  protected readonly options = signal<Supplier[]>([]);
  protected readonly hasMore = signal(false);
  protected readonly loaded = signal(false);

  constructor() {
    this.typing
      .pipe(debounceTime(SEARCH_DEBOUNCE_MS), takeUntilDestroyed())
      .subscribe((term) => this.requests.next({ term, page: 1 }));

    this.requests
      .pipe(
        switchMap((request) =>
          this.service.list(request.page, PAGE_SIZE, request.term).pipe(
            catchError(() => EMPTY),
            map((result) => ({ request, result })),
          ),
        ),
        takeUntilDestroyed(),
      )
      .subscribe(({ request, result }) => {
        this.term = request.term;
        this.page = result.page;
        this.options.update((current) =>
          request.page === 1 ? result.items : [...current, ...result.items],
        );
        this.hasMore.set(result.page < result.totalPages);
        this.loaded.set(true);
      });
  }

  protected displayName(supplier: Supplier | string | null): string {
    return typeof supplier === 'object' && supplier !== null ? supplier.name : '';
  }

  protected onFocus(): void {
    if (!this.loaded()) {
      this.requests.next({ term: '', page: 1 });
    }
  }

  protected onInput(value: string): void {
    this.typing.next(value.trim());
  }

  protected loadMore(): void {
    this.requests.next({ term: this.term, page: this.page + 1 });
  }
}
