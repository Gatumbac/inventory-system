import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-skeleton-rows',
  templateUrl: './skeleton-rows.html',
  styleUrl: './skeleton-rows.scss',
})
export class SkeletonRows {
  readonly widths = input.required<number[]>();
  readonly rows = input(5);

  protected readonly items = computed(() => Array.from({ length: this.rows() }, (_, i) => i));
}
