import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { EMPTY, catchError } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-shell',
  imports: [MatButtonModule, MatToolbarModule, RouterLink, RouterOutlet],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly router = inject(Router);

  protected readonly auth = inject(AuthService);
  protected readonly fullName = signal<string | null>(null);

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
