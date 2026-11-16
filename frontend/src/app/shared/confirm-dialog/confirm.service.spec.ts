import { TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { ConfirmService } from './confirm.service';

const options = { title: '¿Eliminar?', message: 'Se eliminará "Laptop".' };

describe('ConfirmService', () => {
  const ask = (closedWith: boolean | undefined) => {
    const open = vi.fn().mockReturnValue({ afterClosed: () => of(closedWith) });
    TestBed.configureTestingModule({
      providers: [{ provide: MatDialog, useValue: { open } }],
    });
    let result: boolean | undefined;
    TestBed.inject(ConfirmService)
      .ask(options)
      .subscribe((value) => (result = value));
    return { open, result };
  };

  it('emits true when the dialog is confirmed', () => {
    expect(ask(true).result).toBe(true);
  });

  it('emits false when the dialog is cancelled', () => {
    expect(ask(false).result).toBe(false);
  });

  it('emits false when the dialog is dismissed without a result', () => {
    expect(ask(undefined).result).toBe(false);
  });

  it('passes the options as dialog data', () => {
    const { open } = ask(true);

    expect(open.mock.calls[0][1].data).toEqual(options);
  });
});
