import { Component, input, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Subject, debounceTime, distinctUntilChanged, map, skip, startWith } from 'rxjs';

const SEARCH_DEBOUNCE_MS = 300;

@Component({
  selector: 'app-search-field',
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './search-field.html',
  styleUrl: './search-field.scss',
})
export class SearchField {
  private readonly typing = new Subject<string>();

  readonly label = input.required<string>();
  readonly search = output<string>();

  protected readonly value = signal('');

  constructor() {
    this.typing
      .pipe(
        map((value) => value.trim()),
        startWith(''),
        distinctUntilChanged(),
        skip(1),
        debounceTime(SEARCH_DEBOUNCE_MS),
        takeUntilDestroyed(),
      )
      .subscribe((term) => this.search.emit(term));
  }

  protected onInput(value: string): void {
    this.value.set(value);
    this.typing.next(value);
  }

  clear(): void {
    this.onInput('');
  }
}
