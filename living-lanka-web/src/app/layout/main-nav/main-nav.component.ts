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
    <nav class="border-b border-gray-200 bg-white shadow-sm">
      <div class="section-container">
        <div class="flex gap-2 overflow-x-auto py-2.5 scrollbar-hide">
          <a routerLink="/categories" class="ll-nav-chip font-semibold text-maroon-900">
            All Categories
          </a>
          @for (item of navItems(); track item.slug) {
            <a [routerLink]="['/category', item.slug]" class="ll-nav-chip">
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
        cats.slice(0, 12).map((c) => ({
          label: c.title,
          slug: c.slug,
          iconKey: iconKeyFromCategory(c.slug, c.iconUrl ?? c.icon),
        }))
      );
    });
  }
}
