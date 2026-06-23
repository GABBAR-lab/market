import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ListingApiService } from '../../core/services/listing-api.service';
import { PropertyListing } from '../../core/models/marketplace.models';
import { PropertyCardComponent } from '../../shared/components/property-card/property-card.component';

@Component({
  selector: 'app-category',
  imports: [RouterLink, PropertyCardComponent],
  template: `
    <div class="bg-gradient-to-b from-maroon-950 to-maroon-900 py-12 text-white">
      <div class="section-container">
        <nav class="breadcrumbs mb-4 text-sm text-white/70">
          <ul>
            <li><a routerLink="/" class="hover:text-white">Home</a></li>
            <li class="text-white">{{ categoryName() }}</li>
          </ul>
        </nav>
        <h1 class="text-3xl font-bold capitalize">{{ categoryName() }} Listings</h1>
        @if (totalCount() > 0) {
          <p class="mt-2 text-white/70">{{ totalCount() }} ads available</p>
        }
      </div>
    </div>

    <div class="section-container py-10">
      @if (loading()) {
        <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          @for (i of [1,2,3,4]; track i) {
            <div class="skeleton h-80 rounded-2xl"></div>
          }
        </div>
      } @else {
        <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          @for (listing of listings(); track listing.id) {
            <app-property-card [listing]="listing" />
          } @empty {
            <div class="premium-card col-span-full py-16 text-center">
              <p class="text-gray-500">No listings found in this category yet.</p>
              <a routerLink="/post-ad" class="btn mt-4 bg-maroon-800 text-white">Post the First Ad</a>
            </div>
          }
        </div>
      }
    </div>
  `,
})
export class CategoryComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ListingApiService);

  readonly slug = signal('');
  readonly categoryName = signal('');
  readonly categoryId = signal<string | undefined>(undefined);
  readonly listings = signal<PropertyListing[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug') ?? '';
      this.slug.set(slug);

      if (slug === 'all') {
        this.categoryName.set('All');
        this.loadAll();
        return;
      }

      this.api.getCategoryBySlug(slug).subscribe({
        next: (cat) => {
          this.categoryName.set(cat.name);
          this.categoryId.set(cat.id);
          this.loadCategory(cat.id);
        },
        error: () => {
          this.categoryName.set(slug.replace(/-/g, ' '));
          this.loadAll();
        },
      });
    });
  }

  private loadCategory(categoryId: string): void {
    this.loading.set(true);
    this.api.searchListings({ categoryId }, 1, 24).subscribe({
      next: (result) => {
        this.listings.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private loadAll(): void {
    this.loading.set(true);
    this.api.searchListings({}, 1, 24).subscribe({
      next: (result) => {
        this.listings.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
