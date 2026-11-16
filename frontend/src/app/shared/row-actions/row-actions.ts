import { Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-row-actions',
  imports: [MatButtonModule, MatIconModule, MatMenuModule, RouterLink],
  templateUrl: './row-actions.html',
  styleUrl: './row-actions.scss',
})
export class RowActions {
  readonly label = input.required<string>();
  readonly editLink = input<unknown[]>();
  readonly edit = output<void>();
  readonly remove = output<void>();
}
