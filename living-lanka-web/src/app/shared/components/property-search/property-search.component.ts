import { Component, inject, output, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SearchFilters } from '../../../core/models/marketplace.models';

@Component({
  selector: 'app-property-search',
  imports: [FormsModule, RouterLink],
  template: `
    <section class="section-container -mt-10 relative z-10 pb-12">
      <div class="overflow-hidden rounded-3xl bg-base-200 shadow-xl">
        <!-- Tabs -->
        <div class="flex justify-center pt-6">
          <div class="inline-flex overflow-hidden rounded-t-2xl shadow-sm">
            <button
              type="button"
              class="px-8 py-2.5 text-sm font-semibold transition sm:px-12 sm:text-base"
              [class]="activeTab() === 'sale' ? 'bg-white text-gray-800' : 'bg-transparent text-gray-500 hover:text-gray-700'"
              (click)="activeTab.set('sale')"
            >
              For Sale
            </button>
            <button
              type="button"
              class="px-8 py-2.5 text-sm font-semibold text-white transition sm:px-12 sm:text-base"
              [class]="activeTab() === 'rent' ? 'bg-emerald-500' : 'bg-emerald-400/70 hover:bg-emerald-500'"
              (click)="activeTab.set('rent')"
            >
              For Rent
            </button>
          </div>
        </div>

        <!-- Filters -->
        <form class="bg-white p-4 sm:p-6" (ngSubmit)="onSearch()">
          <div class="grid grid-cols-1 gap-3 md:grid-cols-5 md:gap-4">
            <select [(ngModel)]="location" name="location" class="select select-bordered w-full bg-white">
              <option value="">Location</option>
              <option value="colombo">Colombo</option>
              <option value="kandy">Kandy</option>
              <option value="galle">Galle</option>
              <option value="negombo">Negombo</option>
            </select>
            <select [(ngModel)]="propertyType" name="propertyType" class="select select-bordered w-full bg-white">
              <option value="">Choose Property Type</option>
              <option value="house">House</option>
              <option value="apartment">Apartment</option>
              <option value="land">Land</option>
              <option value="commercial">Commercial</option>
            </select>
            <div class="form-control w-full">
              <label class="label py-0">
                <span class="label-text text-xs text-gray-500">Category</span>
              </label>
              <select [(ngModel)]="category" name="category" class="select select-bordered w-full bg-white">
                <option value="residential">Residential</option>
                <option value="commercial">Commercial</option>
              </select>
            </div>
            <select [(ngModel)]="posterType" name="posterType" class="select select-bordered w-full bg-white">
              <option value="">Type Of Poster</option>
              <option value="owner">Owner</option>
              <option value="agent">Agent</option>
            </select>
            <button type="submit" class="btn btn-success w-full gap-2 md:mt-auto">
              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              Search
            </button>
          </div>
        </form>

        <!-- Hero CTA row -->
        <div class="grid grid-cols-1 items-center gap-8 bg-base-200 p-6 sm:p-10 lg:grid-cols-2">
          <div class="overflow-hidden rounded-2xl shadow-lg">
            <img
              src="https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=800&q=80"
              alt="Buy and sell property"
              class="h-56 w-full object-cover sm:h-72"
              loading="lazy"
            />
          </div>
          <div class="text-center lg:text-left">
            <h2 class="text-2xl font-bold text-gray-900 sm:text-3xl lg:text-4xl">
              Buy And Sell Anything Near You
            </h2>
            <p class="mt-4 text-gray-600 sm:text-lg">
              Post ads for free, discover great deals, and connect directly with trusted buyers and sellers across Sri Lanka.
            </p>
            <div class="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-center lg:justify-start">
              <a routerLink="/post-ad" class="btn btn-success">Post Free Ad</a>
              <a routerLink="/search" class="btn btn-warning">Explore Deals</a>
            </div>
          </div>
        </div>
      </div>
    </section>
  `,
})
export class PropertySearchComponent {
  private readonly router = inject(Router);
  readonly search = output<SearchFilters>();
  readonly activeTab = signal<'sale' | 'rent'>('rent');

  location = '';
  propertyType = '';
  category = 'residential';
  posterType = '';

  onSearch(): void {
    const filters: SearchFilters = {
      listingType: this.activeTab(),
      location: this.location,
      city: this.location,
      propertyType: this.propertyType,
      category: this.category,
      posterType: this.posterType,
    };
    this.search.emit(filters);
    this.router.navigate(['/search'], { queryParams: filters });
  }
}
