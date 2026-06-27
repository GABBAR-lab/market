import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { CategoryIconComponent } from '../../shared/components/category-icon/category-icon.component';
import { CategoryAccordionComponent } from '../../shared/components/category-accordion/category-accordion.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { CategoryTreeNode } from '../../core/models/marketplace.models';
import { PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-category',
  imports: [RouterLink, IkmanAdCardComponent, CategoryIconComponent, CategoryAccordionComponent],
  template: `
    <div class="border-b border-gray-200 bg-gray-50 py-8">
      <div class="section-container">
        <nav class="mb-2 text-sm text-gray-500">
          <a routerLink="/" class="hover:text-teal-700">Home</a>
          /
          <a routerLink="/categories" class="hover:text-teal-700">Categories</a>
          / {{ categoryName() }}
        </nav>
        <div class="flex items-center gap-3">
          @if (categoryIcon()) {
            <app-category-icon [iconKey]="categoryIcon()!" [slug]="slug()" size="lg" />
          }
          <div>
            <h1 class="text-2xl font-bold text-gray-900">{{ categoryName() }}</h1>
            <p class="mt-1 text-gray-600">{{ totalCount() }} ads</p>
          </div>
        </div>
      </div>
    </div>

    @if (subcategories().length) {
      <div class="border-b border-gray-100 bg-white py-2">
        <div class="section-container">
          <div class="overflow-hidden rounded-lg border border-gray-200">
            @for (sub of subcategories(); track sub.id) {
              <a
                [routerLink]="subLink(sub)"
                [queryParams]="subParams(sub)"
                class="flex items-center gap-3 border-b border-gray-100 px-4 py-3 text-[#0074ba] last:border-b-0 hover:bg-gray-50"
              >
                @if (sub.iconUrl && sub.iconUrl !== 'all') {
                  <app-category-icon [iconKey]="sub.iconUrl" [slug]="sub.slug" size="sm" />
                } @else {
                  <span class="w-5"></span>
                }
                <span class="text-sm">{{ sub.name }}</span>
              </a>
            }
          </div>
        </div>
      </div>
    }

    <div class="section-container py-10">
      @if (loading()) {
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (i of [1,2,3,4,5,6]; track i) { <div class="skeleton aspect-[4/3] rounded-lg"></div> }
        </div>
      } @else if (listings().length) {
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (listing of listings(); track listing.id) {
            <app-ikman-ad-card [listing]="listing" />
          }
        </div>
      } @else {
        <div class="py-16 text-center">
          <p class="text-gray-500">No ads in this category yet.</p>
          <a routerLink="/post-ad" class="btn mt-4 bg-teal-600 text-white">Post Free Ad</a>
        </div>
      }
    </div>

    <section class="section-container pb-12 lg:hidden">
      <h2 class="mb-4 text-lg font-bold text-gray-900">Browse other categories</h2>
      <app-category-accordion [initialExpanded]="slug()" />
    </section>
  `,
})
export class CategoryComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ListingApiService);

  readonly slug = signal('');
  readonly categoryName = signal('');
  readonly categoryIcon = signal<string | null>(null);
  readonly categoryId = signal<string | undefined>(undefined);
  readonly listings = signal<PropertyListing[]>([]);
  readonly subcategories = signal<CategoryTreeNode[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug') ?? '';
      this.slug.set(slug);

      if (slug === 'all') {
        this.categoryName.set('All Ads');
        this.loadAll();
        return;
      }

      this.api.getCategoryTree().subscribe({
        next: (tree) => {
          const node = tree.find((c) => c.slug === slug);
          if (node) {
            this.categoryName.set(node.name);
            this.categoryIcon.set(node.iconUrl ?? node.slug);
            this.categoryId.set(node.id);
            this.subcategories.set(node.subCategories);
            this.loadCategory(node.id);
            return;
          }

          this.api.getCategoryBySlug(slug).subscribe({
            next: (cat) => {
              this.categoryName.set(cat.name);
              this.categoryIcon.set(cat.iconUrl ?? slug);
              this.categoryId.set(cat.id);
              this.loadCategory(cat.id);
            },
            error: () => {
              this.categoryName.set(slug.replace(/-/g, ' '));
              this.loadAll();
            },
          });
        },
      });
    });
  }

  subLink(sub: CategoryTreeNode): string[] {
    return sub.iconUrl === 'all' || sub.slug.startsWith('all-')
      ? ['/category', this.slug()]
      : ['/search'];
  }

  subParams(sub: CategoryTreeNode): Record<string, string> {
    if (sub.iconUrl === 'all' || sub.slug.startsWith('all-')) {
      return {};
    }
    return { category: this.slug(), query: sub.searchTerm ?? sub.name };
  }

  private loadCategory(categoryId: string): void {
    this.loading.set(true);
    this.api.searchListings({ categoryId }, 1, 24).subscribe({
      next: (r) => {
        this.listings.set(r.items);
        this.totalCount.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private loadAll(): void {
    this.loading.set(true);
    this.api.searchListings({}, 1, 24).subscribe({
      next: (r) => {
        this.listings.set(r.items);
        this.totalCount.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
