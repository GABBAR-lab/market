import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Category } from '../../../core/models/marketplace.models';
import { CategoryIconComponent } from '../category-icon/category-icon.component';
import { iconKeyFromCategory } from '../../../core/data/category-icons';

@Component({
  selector: 'app-category-list-item',
  imports: [RouterLink, CategoryIconComponent],
  template: `
    <a
      [routerLink]="['/category', category.slug]"
      class="group flex items-center gap-3 border-b border-gray-100 px-1 py-3 transition hover:bg-gray-50"
      [class]="variant === 'dark' ? 'hover:bg-white/10 border-white/10' : ''"
    >
      <app-category-icon
        [iconKey]="iconKey"
        [slug]="category.slug"
        [size]="variant === 'compact' ? 'sm' : 'md'"
      />
      <div class="min-w-0 flex-1">
        <p
          class="truncate font-medium text-[#0074ba] group-hover:underline"
          [class]="variant === 'dark' ? 'text-white group-hover:text-amber-400' : ''"
        >
          {{ category.title }}
        </p>
        @if (category.subtitle) {
          <p class="text-sm text-gray-500" [class]="variant === 'dark' ? 'text-white/50' : ''">
            {{ category.subtitle }}
          </p>
        }
      </div>
      <svg class="h-4 w-4 shrink-0 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
      </svg>
    </a>
  `,
})
export class CategoryListItemComponent {
  @Input({ required: true }) category!: Category;
  @Input() variant: 'light' | 'dark' | 'compact' = 'light';

  get iconKey(): string {
    return iconKeyFromCategory(this.category.slug, this.category.iconUrl ?? this.category.icon);
  }
}

@Component({
  selector: 'app-category-icon-card',
  imports: [RouterLink, CategoryIconComponent],
  template: `
    <a
      [routerLink]="['/category', category.slug]"
      class="card group border bg-white shadow-sm transition hover:-translate-y-1 hover:shadow-lg"
      [class]="variant === 'dark' ? 'border-white/10 bg-white/5 hover:border-gold-500/50' : 'border-gray-200 hover:border-teal-400'"
    >
      <div class="card-body items-center p-4 text-center sm:p-6">
        <div class="mb-2 flex h-14 w-14 items-center justify-center">
          <app-category-icon [iconKey]="iconKey" [slug]="category.slug" size="lg" />
        </div>
        <h3 class="text-sm font-bold text-[#0074ba] sm:text-base">{{ category.title }}</h3>
        @if (category.subtitle) {
          <p class="mt-1 text-xs text-gray-500">{{ category.subtitle }}</p>
        }
      </div>
    </a>
  `,
})
export class CategoryIconCardComponent {
  @Input({ required: true }) category!: Category;
  @Input() variant: 'light' | 'dark' = 'light';

  get iconKey(): string {
    return iconKeyFromCategory(this.category.slug, this.category.iconUrl ?? this.category.icon);
  }
}
