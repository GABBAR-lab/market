import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LocationBarComponent } from '../../shared/components/location-bar/location-bar.component';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { CategoryAccordionComponent } from '../../shared/components/category-accordion/category-accordion.component';
import { CategoryIconCardComponent } from '../../shared/components/category-card/category-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { LocaleService } from '../../core/services/locale.service';
import { CATEGORY_QUICK_LINKS } from '../../core/data/category-subcategories';
import { Category, PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-home',
  imports: [RouterLink, FormsModule, LocationBarComponent, IkmanAdCardComponent, CategoryAccordionComponent, CategoryIconCardComponent],
  template: `
    <app-location-bar (locationChange)="onLocationFilter($event)" />

    <section class="border-b border-gray-100 bg-gray-50 py-4">
      <div class="section-container">
        <form class="flex gap-2" (submit)="onSearchSubmit($event)">
          <input
            type="search"
            [(ngModel)]="searchQuery"
            name="q"
            class="input input-bordered flex-1 bg-white"
            placeholder="Search for anything in Sri Lanka..."
          />
          <button type="submit" class="btn bg-maroon-800 text-white">Search</button>
        </form>
      </div>
    </section>

    <section class="section-container py-10">
      <div class="mb-6 flex items-center justify-between">
        <h2 class="text-xl font-bold text-gray-900 sm:text-2xl">{{ locale.label('browse') }}</h2>
        <a routerLink="/categories" class="text-sm font-semibold text-[#0074ba] hover:underline">View all</a>
      </div>

      @if (loading()) {
        <div class="grid grid-cols-2 gap-3 sm:grid-cols-4">
          @for (i of [1,2,3,4,5,6,7,8]; track i) {
            <div class="skeleton h-28 rounded-lg"></div>
          }
        </div>
      } @else {
        <div class="mb-8 hidden grid-cols-2 gap-4 sm:grid md:grid-cols-4 lg:grid-cols-8">
          @for (cat of categories(); track cat.id) {
            <app-category-icon-card [category]="cat" />
          }
        </div>
        <div class="lg:hidden">
          <app-category-accordion />
        </div>
      }
    </section>

    <section class="bg-gray-50 py-10">
      <div class="section-container">
        <h2 class="mb-6 text-xl font-bold text-gray-900 sm:text-2xl">{{ locale.label('featured') }}</h2>
        @if (loading()) {
          <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
            @for (i of [1,2,3,4,5]; track i) {
              <div class="skeleton aspect-[4/3] rounded-lg"></div>
            }
          </div>
        } @else if (featuredListings().length) {
          <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
            @for (listing of featuredListings(); track listing.id) {
              <app-ikman-ad-card [listing]="listing" [showMember]="listing.isFeatured" />
            }
          </div>
        } @else {
          <p class="text-gray-500">No featured ads yet.</p>
        }
      </div>
    </section>

    <section class="section-container py-12">
      <div class="rounded-2xl bg-gradient-to-r from-maroon-900 to-maroon-950 p-8 text-center text-white sm:p-12">
        <h2 class="text-2xl font-bold sm:text-3xl">Start making money!</h2>
        <p class="mx-auto mt-3 max-w-lg text-white/80">
          Do you have something to sell? Post your first ad and start making money!
        </p>
        <a routerLink="/post-ad" class="btn btn-lg mt-6 border-0 bg-amber-400 text-gray-900 hover:bg-amber-300">
          Post your ad
        </a>
      </div>
    </section>

    <section class="border-t border-gray-200 bg-white py-12">
      <div class="section-container">
        <h2 class="mb-8 text-xl font-bold text-gray-900">Quick links</h2>
        <div class="grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-4">
          @for (block of quickLinks; track block.slug) {
            <div>
              <h3 class="mb-3 font-bold text-maroon-900">
                <a [routerLink]="['/category', block.slug]" class="hover:underline">{{ block.title }}</a>
              </h3>
              <ul class="space-y-2 text-sm text-gray-600">
                @for (sub of block.subcategories; track sub.label) {
                  <li>
                    <a
                      [routerLink]="['/search']"
                      [queryParams]="{ query: sub.searchTerm, category: sub.slug }"
                      class="hover:text-maroon-800 hover:underline"
                    >
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

    <section class="section-container pb-16">
      <div class="mb-6 flex items-center justify-between">
        <h2 class="text-xl font-bold text-gray-900">Latest Ads</h2>
        <a routerLink="/all-ads" class="text-sm font-semibold text-maroon-800 hover:underline">View all</a>
      </div>
      <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
        @for (listing of latestListings(); track listing.id) {
          <app-ikman-ad-card [listing]="listing" />
        }
      </div>
    </section>
  `,
})
export class HomeComponent implements OnInit {
  private readonly api = inject(ListingApiService);
  private readonly router = inject(Router);
  readonly locale = inject(LocaleService);

  readonly categories = signal<Category[]>([]);
  readonly featuredListings = signal<PropertyListing[]>([]);
  readonly latestListings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly quickLinks = CATEGORY_QUICK_LINKS;

  searchQuery = '';
  locationFilter = '';

  ngOnInit(): void {
    this.api.getCategories().subscribe({ next: (cats) => this.categories.set(cats) });
    this.loadListings();
  }

  onLocationFilter(location: string): void {
    this.locationFilter = location;
    this.loadListings();
  }

  onSearchSubmit(e: Event): void {
    e.preventDefault();
    this.router.navigate(['/search'], {
      queryParams: {
        query: this.searchQuery || undefined,
        province: this.locationFilter || undefined,
      },
    });
  }

  private loadListings(): void {
    this.loading.set(true);
    const filters = this.locationFilter ? { city: this.locationFilter } : {};

    this.api.getFeaturedListings(10).subscribe({
      next: (featured) => {
        this.featuredListings.set(featured);
        this.api.searchListings(filters, 1, 15).subscribe({
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
