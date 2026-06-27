import { Component, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SRI_LANKA_PROVINCES } from '../../../core/data/sri-lanka-locations';

@Component({
  selector: 'app-location-bar',
  imports: [FormsModule],
  template: `
    <div class="border-b border-gray-200 bg-white">
      <div class="section-container flex flex-wrap items-center gap-3 py-3">
        <svg class="h-5 w-5 text-maroon-800" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.828 0l-4.244-4.243a8 8 0 1111.314 0z" />
        </svg>
        <select
          class="select select-bordered select-sm max-w-xs flex-1 sm:select-md"
          [(ngModel)]="selected"
          (ngModelChange)="onChange()"
        >
          <option value="">All of Sri Lanka</option>
          @for (p of provinces; track p.name) {
            <option [value]="p.name">{{ p.name }}</option>
          }
        </select>
        @if (selected) {
          <button type="button" class="btn btn-ghost btn-xs" (click)="clear()">Clear</button>
        }
      </div>
    </div>
  `,
})
export class LocationBarComponent {
  readonly locationChange = output<string>();
  readonly provinces = SRI_LANKA_PROVINCES;
  selected = '';

  onChange(): void {
    this.locationChange.emit(this.selected);
  }

  clear(): void {
    this.selected = '';
    this.locationChange.emit('');
  }
}
