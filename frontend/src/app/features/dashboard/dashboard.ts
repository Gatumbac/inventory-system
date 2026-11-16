import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { EMPTY, catchError } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { Dashboard as DashboardData } from './dashboard.models';
import { DashboardService } from './dashboard.service';

@Component({
  selector: 'app-dashboard',
  imports: [CurrencyPipe, DecimalPipe, MatIconModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  host: { class: 'page' },
})
export class Dashboard {
  private readonly name = signal<string | null>(null);
  private readonly timeGreeting = this.greetingFor(new Date().getHours());

  protected readonly greeting = computed(() => {
    const name = this.name();
    return name ? `${this.timeGreeting}, ${name}` : this.timeGreeting;
  });
  protected readonly data = signal<DashboardData | null>(null);

  constructor() {
    inject(AuthService)
      .me()
      .pipe(catchError(() => EMPTY))
      .subscribe((user) => this.name.set(user.name));

    inject(DashboardService)
      .get()
      .pipe(catchError(() => EMPTY))
      .subscribe((data) => this.data.set(data));
  }

  private greetingFor(hour: number): string {
    if (hour < 12) {
      return 'Buenos días';
    }

    return hour < 19 ? 'Buenas tardes' : 'Buenas noches';
  }
}
