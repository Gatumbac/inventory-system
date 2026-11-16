import { TestBed } from '@angular/core/testing';
import { SearchField } from './search-field';

describe('SearchField', () => {
  const setup = () => {
    const fixture = TestBed.createComponent(SearchField);
    fixture.componentRef.setInput('label', 'Buscar');
    const emitted: string[] = [];
    fixture.componentInstance.search.subscribe((term) => emitted.push(term));
    fixture.detectChanges();
    return { component: fixture.componentInstance, emitted };
  };

  const type = (component: SearchField, value: string) =>
    (component as unknown as { onInput(value: string): void }).onInput(value);

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [SearchField] });
    vi.useFakeTimers();
  });

  afterEach(() => vi.useRealTimers());

  it('emits once after the debounce with the trimmed term', () => {
    const { component, emitted } = setup();

    type(component, 'la');
    type(component, ' lap ');
    vi.advanceTimersByTime(299);
    expect(emitted).toEqual([]);

    vi.advanceTimersByTime(1);
    expect(emitted).toEqual(['lap']);
  });

  it('does not emit when the term has not changed', () => {
    const { component, emitted } = setup();

    type(component, '   ');
    vi.advanceTimersByTime(300);

    expect(emitted).toEqual([]);
  });

  it('emits an empty term when cleared', () => {
    const { component, emitted } = setup();

    type(component, 'lap');
    vi.advanceTimersByTime(300);
    component.clear();
    vi.advanceTimersByTime(300);

    expect(emitted).toEqual(['lap', '']);
  });
});
