import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { LocationBarComponent } from '../../shared/components/location-bar/location-bar.component';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { CategoryIconCardComponent } from '../../shared/components/category-card/category-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { RecentlyViewedService } from '../../core/services/recently-viewed.service';
import { LocaleService } from '../../core/services/locale.service';
import { CATEGORY_QUICK_LINKS } from '../../core/data/category-subcategories';
import { Category, PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-home',
  imports: [RouterLink, FormsModule, LocationBarComponent, IkmanAdCardComponent, CategoryIconCardComponent],
  template: `
    <app-location-bar (locationChange)="onLocationFilter($event)" />

    <!-- Category grid — OLX style -->
    <section class="border-b border-gray-200 bg-white py-6">
      <div class="section-container">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="text-lg font-bold text-gray-900">{{ locale.label('browse') }}</h2>
          <a routerLink="/categories" class="text-sm font-semibold text-maroon-800 hover:underline">View all</a>
        </div>
        @if (loading()) {
          <div class="grid grid-cols-3 gap-3 sm:grid-cols-4 md:grid-cols-6 lg:grid-cols-8">
            @for (i of [1,2,3,4,5,6,7,8]; track i) {
              <div class="skeleton h-24 rounded-lg"></div>
            }
          </div>
        } @else {
          <div class="grid grid-cols-3 gap-3 sm:grid-cols-4 md:grid-cols-6 lg:grid-cols-8">
            @for (cat of categories(); track cat.id) {
              <app-category-icon-card [category]="cat" />
            }
          </div>
        }
      </div>
    </section>

    @if (recentlyViewed().length) {
      <section class="section-container py-8">
        <h2 class="mb-4 text-lg font-bold text-gray-900">Continue browsing</h2>
        <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (listing of recentlyViewed(); track listing.id) {
            <app-ikman-ad-card [listing]="listing" />
          }
        </div>
      </section>
    }

    <section class="bg-white py-8">
      <div class="section-container">
        <div class="mb-4 flex items-center justify-between">
          <h2 class="text-lg font-bold text-gray-900">{{ locale.label('featured') }}</h2>
        </div>
        @if (loading()) {
          <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
            @for (i of [1,2,3,4,5]; track i) {
              <div class="skeleton aspect-[4/3] rounded-lg"></div>
            }
          </div>
        } @else if (featuredListings().length) {
          <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
            @for (listing of featuredListings(); track listing.id) {
              <app-ikman-ad-card [listing]="listing" />
            }
          </div>
        } @else {
          <p class="text-gray-500">No featured ads yet. <a routerLink="/post-ad" class="font-semibold text-maroon-800">Post the first one!</a></p>
        }
      </div>
    </section>

    <section class="section-container py-8">
      <div class="mb-4 flex items-center justify-between">
        <h2 class="text-lg font-bold text-gray-900">Fresh recommendations</h2>
        <a routerLink="/all-ads" class="text-sm font-semibold text-maroon-800 hover:underline">View all</a>
      </div>
      <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
        @for (listing of latestListings(); track listing.id) {
          <app-ikman-ad-card [listing]="listing" />
        }
      </div>
    </section>

    <section class="section-container pb-8">
      <div class="overflow-hidden rounded-lg bg-maroon-950 p-8 text-center text-white sm:flex sm:items-center sm:justify-between sm:text-left">
        <div>
          <h2 class="text-2xl font-bold">Got something to sell?</h2>
          <p class="mt-2 text-white/70">Post your ad in minutes. Reach buyers across Sri Lanka.</p>
        </div>
        <a routerLink="/post-ad" class="ll-btn-sell mt-6 inline-flex sm:mt-0">Post your ad — It's free</a>
      </div>
    </section>

    <section class="border-t border-gray-200 bg-white py-10">
      <div class="section-container">
        <h2 class="mb-6 text-lg font-bold text-gray-900">Popular searches</h2>
        <div class="grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-4">
          @for (block of quickLinks; track block.slug) {
            <div>
              <h3 class="mb-3 font-bold text-maroon-900">
                <a [routerLink]="['/category', block.slug]" class="hover:underline">{{ block.title }}</a>
              </h3>
              <ul class="space-y-2 text-sm text-gray-600">
                @for (sub of block.subcategories; track sub.label) {
                  <li>
                    <a [routerLink]="['/search']" [queryParams]="{ query: sub.searchTerm }" class="hover:text-maroon-800 hover:underline">
                      {{ sub.label }}
                    </a>
                  </li>
                }
              </ul>
            </div>
          }
        </div>
      </div>
    </section>
  `,
})
export class HomeComponent implements OnInit {
  private readonly api = inject(ListingApiService);
  private readonly router = inject(Router);
  private readonly recentlyViewedSvc = inject(RecentlyViewedService);
  readonly locale = inject(LocaleService);

  readonly categories = signal<Category[]>([]);
  readonly featuredListings = signal<PropertyListing[]>([]);
  readonly latestListings = signal<PropertyListing[]>([]);
  readonly recentlyViewed = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly quickLinks = CATEGORY_QUICK_LINKS;

  locationFilter = '';

  ngOnInit(): void {
    this.api.getCategories().subscribe({ next: (cats) => this.categories.set(cats) });
    this.loadListings();
    this.loadRecentlyViewed();
  }

  private loadRecentlyViewed(): void {
    const ids = this.recentlyViewedSvc.getAll().slice(0, 5);
    if (!ids.length) return;
    forkJoin(ids.map((id) => this.api.getListingById(id).pipe(catchError(() => of(null))))).subscribe((items) => {
      this.recentlyViewed.set(items.filter((x): x is PropertyListing => x !== null));
    });
  }

  onLocationFilter(location: string): void {
    this.locationFilter = location;
    this.loadListings();
  }

  private loadListings(): void {
    this.loading.set(true);
    const filters = this.locationFilter ? { province: this.locationFilter } : {};

    this.api.getFeaturedListings(10).subscribe({
      next: (featured) => {
        this.featuredListings.set(featured);
        this.api.searchListings(filters, 1, 20).subscribe({
          next: (result) => {
            this.latestListings.set(result.items);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
      },
      error: () => this.loading.set(false),
    });
  }
}
