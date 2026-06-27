import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { PropertyListing, SearchFilters } from '../../core/models/marketplace.models';
import { SRI_LANKA_PROVINCES } from '../../core/data/sri-lanka-locations';

const CONDITIONS = ['New', 'Used', 'Refurbished'];

@Component({
  selector: 'app-search',
  imports: [RouterLink, FormsModule, IkmanAdCardComponent],
  template: `
    <div class="ll-page-hero py-8">
      <div class="section-container">
        <h1 class="text-2xl font-bold text-white sm:text-3xl">Search listings</h1>
        <form class="mt-4 flex gap-2" (submit)="applySearch($event)">
          <input
            type="search"
            [(ngModel)]="queryInput"
            name="q"
            class="ll-search-input flex-1"
            placeholder="What are you looking for?"
            aria-label="Search query"
          />
          <button type="submit" class="ll-btn-primary shrink-0 px-6">Search</button>
        </form>
      </div>
    </div>

    <div class="section-container py-8 pb-safe-nav md:pb-12">
      <div class="flex flex-col gap-8 lg:flex-row">
        <!-- Filters sidebar -->
        <aside class="lg:w-64 lg:shrink-0">
          <div class="premium-card sticky top-24 p-5">
            <h2 class="text-sm font-bold uppercase tracking-wide text-gray-500">Filters</h2>

            <div class="mt-4">
              <label class="mb-1.5 block text-sm font-medium text-gray-700">Sort by</label>
              <select class="select-pro" [(ngModel)]="sortBy" (ngModelChange)="applyFilters()" name="sort">
                <option value="newest">Newest first</option>
                <option value="oldest">Oldest first</option>
                <option value="price_asc">Price: Low to High</option>
                <option value="price_desc">Price: High to Low</option>
              </select>
            </div>

            <div class="mt-4">
              <label class="mb-1.5 block text-sm font-medium text-gray-700">Province</label>
              <select class="select-pro" [(ngModel)]="province" (ngModelChange)="applyFilters()" name="province">
                <option value="">All Sri Lanka</option>
                @for (p of provinces; track p.name) {
                  <option [value]="p.name">{{ p.name }}</option>
                }
              </select>
            </div>

            <div class="mt-4">
              <label class="mb-1.5 block text-sm font-medium text-gray-700">Condition</label>
              <select class="select-pro" [(ngModel)]="condition" (ngModelChange)="applyFilters()" name="condition">
                <option value="">Any condition</option>
                @for (c of conditions; track c) {
                  <option [value]="c">{{ c }}</option>
                }
              </select>
            </div>

            <div class="mt-4">
              <label class="mb-1.5 block text-sm font-medium text-gray-700">Min price (Rs)</label>
              <input type="number" class="input-pro" [(ngModel)]="minPrice" (ngModelChange)="applyFilters()" name="minPrice" min="0" placeholder="0" />
            </div>

            <div class="mt-4">
              <label class="mb-1.5 block text-sm font-medium text-gray-700">Max price (Rs)</label>
              <input type="number" class="input-pro" [(ngModel)]="maxPrice" (ngModelChange)="applyFilters()" name="maxPrice" min="0" placeholder="Any" />
            </div>

            <button type="button" class="btn-brand-outline mt-6 w-full text-sm" (click)="clearFilters()">Clear filters</button>
          </div>
        </aside>

        <!-- Results -->
        <div class="min-w-0 flex-1">
          @if (!loading() && totalCount() >= 0) {
            <p class="mb-6 text-sm text-gray-600">
              <span class="font-semibold text-gray-900">{{ totalCount() }}</span> results found
            </p>
          }

          @if (loading()) {
            <div class="grid grid-cols-2 gap-4 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4">
              @for (i of [1,2,3,4,5,6,7,8]; track i) {
                <div class="skeleton aspect-[4/3] rounded-xl"></div>
              }
            </div>
          } @else if (listings().length) {
            <div class="grid grid-cols-2 gap-4 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4">
              @for (listing of listings(); track listing.id) {
                <app-ikman-ad-card [listing]="listing" />
              }
            </div>
            @if (totalPages() > 1) {
              <div class="mt-10 flex items-center justify-center gap-3">
                <button type="button" class="btn-brand-outline px-4 py-2 text-sm" [disabled]="page() <= 1" (click)="goPage(page() - 1)">Previous</button>
                <span class="text-sm text-gray-600">Page {{ page() }} of {{ totalPages() }}</span>
                <button type="button" class="btn-brand-outline px-4 py-2 text-sm" [disabled]="page() >= totalPages()" (click)="goPage(page() + 1)">Next</button>
              </div>
            }
          } @else {
            <div class="premium-card py-16 text-center">
              <div class="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-gray-100">
                <svg class="h-7 w-7 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
              <h2 class="text-lg font-bold text-gray-900">No listings found</h2>
              <p class="mt-2 text-sm text-gray-500">Try different keywords or adjust your filters.</p>
              <a routerLink="/post-ad" class="btn-brand mt-6 inline-flex">Post an ad</a>
            </div>
          }
        </div>
      </div>
    </div>
  `,
})
export class SearchComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ListingApiService);

  readonly listings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly page = signal(1);
  readonly totalPages = signal(1);
  readonly totalCount = signal(0);

  queryInput = '';
  sortBy: SearchFilters['sortBy'] = 'newest';
  province = '';
  condition = '';
  minPrice?: number;
  maxPrice?: number;
  readonly provinces = SRI_LANKA_PROVINCES;
  readonly conditions = CONDITIONS;
  private filters: SearchFilters = {};

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.queryInput = params.get('query') ?? '';
      this.sortBy = (params.get('sortBy') as SearchFilters['sortBy']) ?? 'newest';
      this.province = params.get('province') ?? '';
      this.condition = params.get('condition') ?? '';
      const min = params.get('minPrice');
      const max = params.get('maxPrice');
      this.minPrice = min ? Number(min) : undefined;
      this.maxPrice = max ? Number(max) : undefined;

      this.filters = {
        query: this.queryInput || undefined,
        province: this.province || undefined,
        condition: this.condition || undefined,
        city: params.get('city') ?? undefined,
        category: params.get('category') ?? undefined,
        categoryId: params.get('categoryId') ?? undefined,
        listingType: (params.get('listingType') as 'sale' | 'rent') ?? undefined,
        minPrice: this.minPrice,
        maxPrice: this.maxPrice,
        sortBy: this.sortBy,
      };
      this.page.set(1);
      this.search();
    });
  }

  applySearch(e: Event): void {
    e.preventDefault();
    this.router.navigate(['/search'], {
      queryParams: {
        query: this.queryInput || null,
        sortBy: this.sortBy,
        province: this.province || null,
        condition: this.condition || null,
        minPrice: this.minPrice ?? null,
        maxPrice: this.maxPrice ?? null,
      },
      queryParamsHandling: 'merge',
    });
  }

  applyFilters(): void {
    this.router.navigate(['/search'], {
      queryParams: {
        sortBy: this.sortBy,
        province: this.province || null,
        condition: this.condition || null,
        minPrice: this.minPrice ?? null,
        maxPrice: this.maxPrice ?? null,
      },
      queryParamsHandling: 'merge',
    });
  }

  clearFilters(): void {
    this.sortBy = 'newest';
    this.province = '';
    this.condition = '';
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.router.navigate(['/search'], { queryParams: { query: this.queryInput || null } });
  }

  goPage(p: number): void {
    this.page.set(p);
    this.search();
    window.scrollTo({ top: 0, behavior: 'smooth' });
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
