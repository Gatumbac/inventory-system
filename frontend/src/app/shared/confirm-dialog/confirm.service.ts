import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable, map } from 'rxjs';
import { ConfirmDialog, ConfirmOptions } from './confirm-dialog';

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly dialog = inject(MatDialog);

  ask(options: ConfirmOptions): Observable<boolean> {
    return this.dialog
      .open<ConfirmDialog, ConfirmOptions, boolean>(ConfirmDialog, {
        data: options,
        width: '400px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .pipe(map((confirmed) => confirmed === true));
  }
}
