import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { HeroSectionComponent } from '../../shared/components/hero-section/hero-section.component';
import { PropertySearchComponent } from '../../shared/components/property-search/property-search.component';
import {
  CategoryIconCardComponent,
  CategoryListItemComponent,
} from '../../shared/components/category-card/category-card.component';
import {
  PropertyCardComponent,
  ListingCardComponent,
} from '../../shared/components/property-card/property-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { Category, PropertyListing, SearchFilters } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-home',
  imports: [
    RouterLink,
    HeroSectionComponent,
    PropertySearchComponent,
    CategoryIconCardComponent,
    CategoryListItemComponent,
    PropertyCardComponent,
    ListingCardComponent,
  ],
  template: `
    <app-hero-section (search)="onSearch($event)" />

    <app-property-search (search)="onSearch($event)" />

    <!-- Stats bar -->
    <section class="relative z-10 -mt-6">
      <div class="section-container">
        <div class="premium-card grid grid-cols-2 gap-4 p-6 sm:grid-cols-4 sm:gap-6 sm:p-8">
          @for (stat of stats; track stat.label) {
            <div class="text-center">
              <p class="text-2xl font-extrabold text-maroon-900 sm:text-3xl">{{ stat.value }}</p>
              <p class="mt-1 text-xs font-medium uppercase tracking-wider text-gray-500 sm:text-sm">{{ stat.label }}</p>
            </div>
          }
        </div>
      </div>
    </section>

    <!-- Quick category icons -->
    <section class="section-container py-14">
      <div class="mb-8 flex items-end justify-between">
        <div>
          <p class="text-sm font-semibold uppercase tracking-widest text-gold-600">Browse</p>
          <h2 class="text-2xl font-bold text-gray-900 sm:text-3xl">Popular Categories</h2>
        </div>
      </div>
      @if (loading()) {
        <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 lg:gap-4">
          @for (i of [1,2,3,4,5,6,7,8,9,10]; track i) {
            <div class="skeleton h-28 rounded-2xl"></div>
          }
        </div>
      } @else {
        <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 lg:gap-4">
          @for (cat of quickCategories(); track cat.id) {
            <app-category-icon-card [category]="cat" />
          }
        </div>
      }
    </section>

    <!-- Search by categories list -->
    <section class="bg-gradient-to-b from-maroon-950 via-maroon-900 to-maroon-950 py-16 text-white">
      <div class="section-container">
        <p class="text-sm font-semibold uppercase tracking-widest text-gold-400">Explore</p>
        <h2 class="mb-10 text-2xl font-bold sm:text-3xl">Search Items By Categories</h2>
        <div class="grid grid-cols-1 gap-1 sm:grid-cols-2 lg:grid-cols-4 lg:gap-2">
          @for (cat of categories(); track cat.id) {
            <app-category-list-item [category]="cat" variant="dark" />
          }
        </div>
      </div>
    </section>

    <!-- Featured Properties -->
    <section class="section-container py-16">
      <div class="mb-10 text-center">
        <p class="text-sm font-semibold uppercase tracking-widest text-gold-600">Premium Picks</p>
        <h2 class="text-2xl font-bold text-gray-900 sm:text-3xl">Featured Properties</h2>
      </div>
      <div class="rounded-3xl bg-gradient-to-br from-maroon-900 via-maroon-950 to-black p-4 shadow-2xl ring-1 ring-gold-500/20 sm:p-8 lg:p-10">
        @if (loading()) {
          <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
            @for (i of [1,2,3,4]; track i) {
              <div class="skeleton h-80 rounded-2xl"></div>
            }
          </div>
        } @else if (featuredListings().length) {
          <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
            @for (listing of featuredListings(); track listing.id) {
              <app-property-card [listing]="listing" />
            }
          </div>
        } @else {
          <p class="py-12 text-center text-white/70">No featured listings yet. Be the first to post!</p>
        }
      </div>
    </section>

    <!-- Featured Listings -->
    <section class="section-container pb-20">
      <div class="mb-10 flex items-end justify-between">
        <div>
          <p class="text-sm font-semibold uppercase tracking-widest text-gold-600">Latest</p>
          <h2 class="text-2xl font-bold text-gray-900 sm:text-3xl">Fresh Listings</h2>
        </div>
        <a routerLink="/search" class="btn btn-outline btn-sm border-maroon-800 text-maroon-800">View All</a>
      </div>
      <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        @for (listing of featuredListings(); track listing.id) {
          <app-listing-card [listing]="listing" />
        }
      </div>
    </section>
  `,
})
export class HomeComponent implements OnInit {
  private readonly api = inject(ListingApiService);
  private readonly router = inject(Router);

  readonly categories = signal<Category[]>([]);
  readonly quickCategories = signal<Category[]>([]);
  readonly featuredListings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);

  readonly stats = [
    { value: '16+', label: 'Categories' },
    { value: '50K+', label: 'Active Ads' },
    { value: '25', label: 'Districts' },
    { value: 'Free', label: 'To Post' },
  ];

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats);
        this.quickCategories.set(cats.slice(0, 10));
        this.stats[0].value = `${cats.length}+`;
      },
      error: () => this.loading.set(false),
    });

    this.api.getFeaturedListings(8).subscribe({
      next: (listings) => {
        this.featuredListings.set(listings);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onSearch(filters: SearchFilters): void {
    this.router.navigate(['/search'], { queryParams: filters });
  }
}
