import { Component, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { EMPTY, catchError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-shell',
  imports: [
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly router = inject(Router);

  protected readonly auth = inject(AuthService);
  protected readonly fullName = signal<string | null>(null);
  protected readonly navOpen = signal(false);
  protected readonly initial = computed(() =>
    (this.fullName() ?? this.auth.email() ?? '?').charAt(0).toUpperCase(),
  );

  constructor() {
    this.auth
      .me()
      .pipe(catchError(() => EMPTY))
      .subscribe((user) => this.fullName.set(`${user.name} ${user.lastName}`));
  }

  protected logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
