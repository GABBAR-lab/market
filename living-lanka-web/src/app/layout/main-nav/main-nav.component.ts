import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NavItem } from '../../core/models/marketplace.models';
import { ListingApiService } from '../../core/services/listing-api.service';

@Component({
  selector: 'app-main-nav',
  imports: [RouterLink],
  template: `
    <nav class="border-b border-gray-200 bg-gradient-to-r from-base-200 to-white">
      <div class="section-container">
        <ul class="hidden items-center gap-1 py-2 lg:flex">
          <li>
            <a routerLink="/" class="nav-link font-semibold">Home</a>
          </li>
          @for (item of navItems(); track item.slug) {
            <li class="dropdown dropdown-hover">
              <a
                [routerLink]="['/category', item.slug]"
                tabindex="0"
                class="nav-link"
                [class.nav-link-active]="item.slug === activeSlug"
              >
                {{ item.label }}
                @if (item.children?.length) {
                  <svg class="h-3.5 w-3.5 opacity-60" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                  </svg>
                }
              </a>
              @if (item.children?.length) {
                <ul tabindex="0" class="dropdown-content menu z-50 mt-0 w-52 rounded-box bg-base-100 p-2 shadow-xl ring-1 ring-black/5">
                  @for (child of item.children; track child.slug) {
                    <li><a [routerLink]="['/category', child.slug]">{{ child.label }}</a></li>
                  }
                </ul>
              }
            </li>
          }
        </ul>

        <div class="flex gap-2 overflow-x-auto py-2 lg:hidden">
          <a routerLink="/" class="whitespace-nowrap rounded-full bg-maroon-800 px-4 py-1.5 text-sm font-medium text-white shadow-sm">Home</a>
          @for (item of navItems(); track item.slug) {
            <a
              [routerLink]="['/category', item.slug]"
              class="whitespace-nowrap rounded-full border border-gray-200 bg-white px-4 py-1.5 text-sm font-medium text-maroon-800 shadow-sm"
            >
              {{ item.label }}
            </a>
          }
        </div>
      </div>
    </nav>
  `,
})
export class MainNavComponent implements OnInit {
  @Input() activeSlug = 'all';

  private readonly api = inject(ListingApiService);
  readonly navItems = signal<NavItem[]>([
    { label: 'All Ads', slug: 'all' },
  ]);

  ngOnInit(): void {
    this.api.getCategories().subscribe((cats) => {
      const items: NavItem[] = [
        { label: 'All Ads', slug: 'all' },
        ...cats.slice(0, 8).map((c) => ({ label: c.title, slug: c.slug })),
      ];
      this.navItems.set(items);
    });
  }
}
