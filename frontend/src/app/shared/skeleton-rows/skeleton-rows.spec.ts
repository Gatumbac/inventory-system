import { TestBed } from '@angular/core/testing';
import { SkeletonRows } from './skeleton-rows';

describe('SkeletonRows', () => {
  const render = (widths: number[], rows?: number) => {
    const fixture = TestBed.createComponent(SkeletonRows);
    fixture.componentRef.setInput('widths', widths);
    if (rows !== undefined) {
      fixture.componentRef.setInput('rows', rows);
    }
    fixture.detectChanges();
    return fixture.nativeElement as HTMLElement;
  };

  it('renders five rows by default with one bar per width', () => {
    const element = render([10, 20, 30]);

    expect(element.querySelectorAll('.row').length).toBe(5);
    expect(element.querySelectorAll('.row .bar').length).toBe(15);
  });

  it('renders the requested number of rows', () => {
    expect(render([40, 60], 3).querySelectorAll('.row').length).toBe(3);
  });

  it('announces itself as a loading status', () => {
    const status = render([10]).querySelector('[role="status"]');

    expect(status?.getAttribute('aria-label')).toBe('Cargando');
  });
});
