import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ListingApiService } from '../../core/services/listing-api.service';
import { NavItem } from '../../core/models/marketplace.models';
import { CategoryIconComponent } from '../../shared/components/category-icon/category-icon.component';
import { iconKeyFromCategory } from '../../core/data/category-icons';

@Component({
  selector: 'app-main-nav',
  imports: [RouterLink, CategoryIconComponent],
  template: `
    <nav class="border-b border-gray-200 bg-white">
      <div class="section-container">
        <div class="flex gap-1 overflow-x-auto py-2">
          <a
            routerLink="/categories"
            class="flex shrink-0 items-center gap-1.5 whitespace-nowrap rounded-full px-3 py-1.5 text-sm font-semibold text-[#0074ba] hover:bg-teal-50"
          >
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
            Categories
          </a>
          <a
            routerLink="/all-ads"
            class="whitespace-nowrap rounded-full px-4 py-1.5 text-sm font-semibold text-gray-700 hover:bg-teal-50 hover:text-teal-700"
          >
            All Ads
          </a>
          @for (item of navItems(); track item.slug) {
            <a
              [routerLink]="['/category', item.slug]"
              class="flex shrink-0 items-center gap-1.5 whitespace-nowrap rounded-full px-3 py-1.5 text-sm text-gray-700 hover:bg-teal-50 hover:text-teal-700"
            >
              <app-category-icon [iconKey]="item.iconKey" [slug]="item.slug" size="sm" />
              {{ item.label }}
            </a>
          }
        </div>
      </div>
    </nav>
  `,
})
export class MainNavComponent implements OnInit {
  private readonly api = inject(ListingApiService);
  readonly navItems = signal<(NavItem & { iconKey: string })[]>([]);

  ngOnInit(): void {
    this.api.getCategories().subscribe((cats) => {
      this.navItems.set(
        cats.slice(0, 10).map((c) => ({
          label: c.title,
          slug: c.slug,
          iconKey: iconKeyFromCategory(c.slug, c.iconUrl ?? c.icon),
        }))
      );
    });
  }
}
