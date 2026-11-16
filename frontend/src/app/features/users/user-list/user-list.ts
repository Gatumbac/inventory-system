import { NgTemplateOutlet } from '@angular/common';
import { Component, inject, signal, viewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorIntl, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { EMPTY, catchError, filter, finalize, switchMap } from 'rxjs';
import { SpanishPaginatorIntl } from '../../../core/layout/paginator-intl';
import { NotificationService } from '../../../core/notifications/notification.service';
import { ConfirmService } from '../../../shared/confirm-dialog/confirm.service';
import { SearchField } from '../../../shared/search-field/search-field';
import { SkeletonRows } from '../../../shared/skeleton-rows/skeleton-rows';
import { User } from '../user.models';
import { UserService } from '../user.service';

@Component({
  selector: 'app-user-list',
  imports: [
    NgTemplateOutlet,
    MatButtonModule,
    MatIconModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatTableModule,
    RouterLink,
    SearchField,
    SkeletonRows,
  ],
  templateUrl: './user-list.html',
  host: { class: 'page' },
  providers: [{ provide: MatPaginatorIntl, useClass: SpanishPaginatorIntl }],
})
export class UserList {
  private readonly service = inject(UserService);
  private readonly notifications = inject(NotificationService);
  private readonly confirm = inject(ConfirmService);

  protected readonly searchField = viewChild.required(SearchField);

  protected readonly columns = ['name', 'email', 'role', 'actions'];
  protected readonly pageSizeOptions = [5, 10, 25, 50];
  protected readonly roleLabels: Record<string, string> = {
    Admin: 'Administrador',
    Viewer: 'Solo lectura',
  };
  protected readonly users = signal<User[]>([]);
  protected readonly loading = signal(true);
  protected readonly failed = signal(false);
  protected readonly page = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly search = signal('');
  protected readonly totalCount = signal(0);

  constructor() {
    this.load();
  }

  protected onPage(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.load();
  }

  protected onSearch(value: string): void {
    this.search.set(value.trim());
    this.page.set(1);
    this.load();
  }

  protected deactivate(user: User): void {
    this.confirm
      .ask({
        title: '¿Desactivar usuario?',
        message: `${user.name} ${user.lastName} ya no podrá iniciar sesión.`,
        confirmLabel: 'Desactivar',
        destructive: true,
      })
      .pipe(
        filter(Boolean),
        switchMap(() => this.service.remove(user.id)),
        catchError(() => EMPTY),
      )
      .subscribe(() => {
        this.notifications.success('Usuario desactivado');
        if (this.users().length === 1 && this.page() > 1) {
          this.page.update((page) => page - 1);
        }
        this.load();
      });
  }

  private load(): void {
    this.loading.set(true);
    this.failed.set(false);
    this.service
      .list(this.page(), this.pageSize(), this.search())
      .pipe(
        catchError(() => {
          this.failed.set(true);
          return EMPTY;
        }),
        finalize(() => this.loading.set(false)),
      )
      .subscribe((result) => {
        this.users.set(result.items);
        this.totalCount.set(result.totalCount);
      });
  }
}
