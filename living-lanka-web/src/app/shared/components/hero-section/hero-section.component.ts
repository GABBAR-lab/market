import { Component, inject, output, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SearchFilters } from '../../../core/models/marketplace.models';

@Component({
  selector: 'app-hero-section',
  imports: [FormsModule],
  template: `
    <section class="relative min-h-[420px] overflow-hidden sm:min-h-[500px] lg:min-h-[560px]">
      <div class="absolute inset-0 bg-hero-beach bg-cover bg-center"></div>
      <div class="absolute inset-0 bg-gradient-to-b from-black/50 via-black/40 to-black/60"></div>

      <div class="relative flex min-h-[inherit] flex-col items-center justify-center px-4 py-16 text-center">
        <h1
          class="mb-8 max-w-4xl text-2xl font-extrabold uppercase tracking-wide text-white drop-shadow-lg sm:text-4xl lg:text-5xl"
        >
          Find Your Perfect Space In Paradise
        </h1>

        <form
          class="w-full max-w-4xl rounded-2xl bg-white/95 p-2 shadow-2xl backdrop-blur-sm sm:rounded-full sm:p-1.5"
          (ngSubmit)="onSearch()"
        >
          <div class="flex flex-col gap-2 sm:flex-row sm:items-center">
            <input
              type="text"
              [(ngModel)]="query"
              name="query"
              placeholder="Search for property, vehicles, goods..."
              class="input input-ghost flex-1 border-0 bg-transparent text-base focus:outline-none sm:pl-6"
            />
            <select
              [(ngModel)]="category"
              name="category"
              class="select select-bordered select-sm w-full border-gray-200 sm:select-md sm:w-40"
            >
              <option value="">Categories</option>
              <option value="property">Property</option>
              <option value="vehicles">Vehicles</option>
              <option value="electronics">Electronics</option>
              <option value="jobs">Jobs</option>
            </select>
            <select
              [(ngModel)]="listingType"
              name="listingType"
              class="select select-bordered select-sm w-full border-gray-200 sm:select-md sm:w-36"
            >
              <option value="sale">Buy</option>
              <option value="rent">Lease</option>
            </select>
            <button type="submit" class="btn btn-warning rounded-xl px-6 sm:rounded-full">
              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <span class="sm:hidden">Search</span>
            </button>
          </div>
        </form>
      </div>
    </section>
  `,
})
export class HeroSectionComponent {
  private readonly router = inject(Router);
  readonly search = output<SearchFilters>();

  query = '';
  category = '';
  listingType: 'sale' | 'rent' = 'sale';

  onSearch(): void {
    const filters: SearchFilters = {
      query: this.query,
      category: this.category,
      listingType: this.listingType,
    };
    this.search.emit(filters);
    this.router.navigate(['/search'], { queryParams: filters });
  }
}
