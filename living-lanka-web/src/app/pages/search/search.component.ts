import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PropertyCardComponent } from '../../shared/components/property-card/property-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { PropertyListing, SearchFilters } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-search',
  imports: [RouterLink, PropertyCardComponent],
  template: `
    <div class="bg-gradient-to-b from-maroon-950 to-maroon-900 py-12 text-white">
      <div class="section-container">
        <p class="text-sm font-semibold uppercase tracking-widest text-gold-400">Search Results</p>
        <h1 class="text-3xl font-bold">Find What You Need</h1>
        @if (totalCount() > 0) {
          <p class="mt-2 text-white/70">{{ totalCount() }} listings found</p>
        }
      </div>
    </div>

    <div class="section-container py-10">
      @if (loading()) {
        <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          @for (i of [1,2,3,4,5,6,7,8]; track i) {
            <div class="skeleton h-80 rounded-2xl"></div>
          }
        </div>
      } @else if (listings().length) {
        <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          @for (listing of listings(); track listing.id) {
            <app-property-card [listing]="listing" />
          }
        </div>

        @if (totalPages() > 1) {
          <div class="mt-10 flex justify-center gap-2">
            <button type="button" class="btn btn-sm" [disabled]="page() <= 1" (click)="goPage(page() - 1)">Prev</button>
            <span class="flex items-center px-4 text-sm text-gray-600">Page {{ page() }} of {{ totalPages() }}</span>
            <button type="button" class="btn btn-sm" [disabled]="page() >= totalPages()" (click)="goPage(page() + 1)">Next</button>
          </div>
        }
      } @else {
        <div class="premium-card py-16 text-center">
          <p class="text-gray-500">No listings match your search.</p>
          <a routerLink="/" class="btn mt-4 bg-maroon-800 text-white">Back to Home</a>
        </div>
      }
    </div>
  `,
})
export class SearchComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ListingApiService);

  readonly listings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly page = signal(1);
  readonly totalPages = signal(1);
  readonly totalCount = signal(0);
  private filters: SearchFilters = {};

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.filters = {
        query: params.get('query') ?? undefined,
        location: params.get('location') ?? undefined,
        city: params.get('city') ?? undefined,
        category: params.get('category') ?? undefined,
        categoryId: params.get('categoryId') ?? undefined,
        listingType: (params.get('listingType') as 'sale' | 'rent') ?? undefined,
        propertyType: params.get('propertyType') ?? undefined,
      };
      this.page.set(1);
      this.search();
    });
  }

  goPage(p: number): void {
    this.page.set(p);
    this.search();
  }

  private search(): void {
    this.loading.set(true);
    this.api.searchListings(this.filters, this.page(), 20).subscribe({
      next: (result) => {
        this.listings.set(result.items);
        this.totalPages.set(result.totalPages);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.listings.set([]);
        this.loading.set(false);
      },
    });
  }
}
