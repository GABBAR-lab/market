import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Category } from '../../../core/models/marketplace.models';

const ICON_PATHS: Record<string, string> = {
  car: 'M8 17h8M5 11l1-4h12l1 4M6 17a1 1 0 100-2 1 1 0 000 2zm12 0a1 1 0 100-2 1 1 0 000 2zM7 11h10',
  phone: 'M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z',
  home: 'M3 12l9-9 9 9M5 10v10h14V10',
  monitor: 'M4 6h16v10H4V6zm4 14h8',
  sofa: 'M4 10h16v6H4v-6zm2-4h12v4H6V6z',
  wrench: 'M14.7 6.3a4 4 0 00-5.4 5.4L4 17v3h3l5.3-5.3a4 4 0 005.4-5.4z',
  building: 'M4 21V5a1 1 0 011-1h5v17M15 21V9h4a1 1 0 011 1v11M9 9h2M9 13h2M9 17h2',
  briefcase: 'M8 7V5a2 2 0 012-2h4a2 2 0 012 2v2m-10 0h10a2 2 0 012 2v8a2 2 0 01-2 2H6a2 2 0 01-2-2V9a2 2 0 012-2z',
  ball: 'M12 22a10 10 0 100-20 10 10 0 000 20zM12 6v12M6 12h12',
  paw: 'M12 11c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM6 13c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM18 13c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2z',
  shirt: 'M16 3l4 4-2 2v12H6V9L4 7l4-4 4 2 4-2z',
  graduation: 'M12 3L2 8l10 5 10-5-10-5zM4 10v6l8 4 8-4v-6',
  bookmark: 'M6 4h12v16l-6-4-6 4V4z',
  bag: 'M6 6h12l1 14H5L6 6zm3-2h6l1 2H8l1-2z',
  leaf: 'M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z',
  globe: 'M12 22a10 10 0 100-20 10 10 0 000 20zM2 12h20M12 2a15 15 0 010 20M12 2a15 15 0 000 20',
};

@Component({
  selector: 'app-category-list-item',
  imports: [RouterLink],
  template: `
    <a
      [routerLink]="['/category', category.slug]"
      class="group flex items-center gap-3 rounded-xl p-3 transition"
      [class]="variant === 'dark' ? 'hover:bg-white/10' : 'hover:bg-base-200'"
    >
      <div
        class="flex h-11 w-11 shrink-0 items-center justify-center rounded-lg transition group-hover:scale-105"
        [class]="variant === 'dark'
          ? 'bg-white/10 text-gold-400 ring-1 ring-gold-500/30'
          : 'bg-gradient-to-br from-blue-50 to-indigo-100 text-indigo-600'"
      >
        <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" [attr.d]="iconPath" />
        </svg>
      </div>
      <div class="min-w-0">
        <p class="truncate font-semibold group-hover:text-gold-400" [class]="variant === 'dark' ? 'text-white' : 'text-gray-800 group-hover:text-maroon-800'">
          {{ category.title }}
        </p>
        @if (category.subtitle) {
          <p class="text-sm" [class]="variant === 'dark' ? 'text-white/50' : 'text-gray-400'">{{ category.subtitle }}</p>
        }
      </div>
    </a>
  `,
})
export class CategoryListItemComponent {
  @Input({ required: true }) category!: Category;
  @Input() variant: 'light' | 'dark' = 'light';

  get iconPath(): string {
    return ICON_PATHS[this.category.icon] ?? ICON_PATHS['bookmark'];
  }
}

@Component({
  selector: 'app-category-icon-card',
  imports: [RouterLink],
  template: `
    <a
      [routerLink]="['/category', category.slug]"
      class="card group border bg-white shadow-sm transition hover:-translate-y-1 hover:shadow-lg"
      [class]="variant === 'dark' ? 'border-white/10 bg-white/5 hover:border-gold-500/50' : 'border-gray-200 hover:border-gold-400'"
    >
      <div class="card-body items-center p-4 text-center sm:p-6">
        <div
          class="mb-2 flex h-12 w-12 items-center justify-center rounded-full border-2 transition sm:h-14 sm:w-14"
          [class]="variant === 'dark'
            ? 'border-gold-500/50 text-gold-400 group-hover:bg-gold-500/10'
            : 'border-gold-500 text-gold-600 group-hover:bg-gold-50'"
        >
          <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" [attr.d]="iconPath" />
          </svg>
        </div>
        <h3 class="text-sm font-bold sm:text-base" [class]="variant === 'dark' ? 'text-white' : 'text-gray-900'">{{ category.title }}</h3>
      </div>
    </a>
  `,
})
export class CategoryIconCardComponent {
  @Input({ required: true }) category!: Category;
  @Input() variant: 'light' | 'dark' = 'light';

  get iconPath(): string {
    return ICON_PATHS[this.category.icon] ?? ICON_PATHS['bookmark'];
  }
}
