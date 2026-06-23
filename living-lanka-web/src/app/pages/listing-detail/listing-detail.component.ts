import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ListingApiService } from '../../core/services/listing-api.service';
import { PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-listing-detail',
  imports: [RouterLink, DecimalPipe],
  template: `
    @if (loading()) {
      <div class="section-container py-20">
        <div class="skeleton h-[500px] w-full rounded-2xl"></div>
      </div>
    } @else if (error()) {
      <div class="section-container py-20 text-center">
        <p class="text-gray-500">{{ error() }}</p>
        <a routerLink="/" class="btn mt-4 bg-maroon-800 text-white">Back Home</a>
      </div>
    } @else if (listing(); as item) {
      <div class="bg-gradient-to-b from-maroon-950 to-maroon-900 py-8 text-white">
        <div class="section-container">
          <nav class="breadcrumbs text-sm text-white/70">
            <ul>
              <li><a routerLink="/" class="hover:text-white">Home</a></li>
              <li><a [routerLink]="['/category', categorySlug()]" class="hover:text-white">{{ item.category }}</a></li>
              <li class="text-white">{{ item.title }}</li>
            </ul>
          </nav>
        </div>
      </div>

      <div class="section-container -mt-4 py-10">
        <div class="grid grid-cols-1 gap-8 lg:grid-cols-5">
          <div class="lg:col-span-3">
            <div class="overflow-hidden rounded-3xl shadow-2xl ring-1 ring-black/5">
              <img [src]="item.imageUrl" [alt]="item.title" class="w-full object-cover lg:h-[520px]" />
            </div>
          </div>
          <div class="lg:col-span-2">
            <div class="premium-card sticky top-4 p-6 sm:p-8">
              @if (item.isFeatured) {
                <span class="badge border-0 bg-gold-500 text-black">Featured</span>
              }
              <h1 class="mt-2 text-2xl font-bold text-gray-900 sm:text-3xl">{{ item.title }}</h1>
              <p class="mt-3 text-3xl font-extrabold text-maroon-800">
                Rs {{ item.price | number:'1.0-0' }}
                <span class="text-base font-normal text-gray-500">{{ item.currency }} / {{ item.priceType }}</span>
              </p>
              <p class="mt-2 flex items-center gap-2 text-gray-500">
                <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.828 0l-4.244-4.243a8 8 0 1111.314 0z" />
                </svg>
                {{ item.location }}
              </p>

              <div class="mt-4 flex flex-wrap gap-2">
                <span class="badge badge-outline">{{ item.condition }}</span>
                <span class="badge badge-outline">{{ item.status }}</span>
                @if (item.viewCount) {
                  <span class="badge badge-ghost">{{ item.viewCount }} views</span>
                }
              </div>

              <div class="divider"></div>

              @if (item.showPhone && item.contactPhone) {
                <a [href]="'tel:' + item.contactPhone" class="btn btn-success btn-block gap-2">
                  <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                  </svg>
                  Call {{ item.contactPhone }}
                </a>
              }
              @if (item.showEmail && item.contactEmail) {
                <a [href]="'mailto:' + item.contactEmail" class="btn btn-outline btn-block mt-2">Email Seller</a>
              }
            </div>
          </div>
        </div>

        <div class="premium-card mt-8 p-6 sm:p-8">
          <h2 class="text-xl font-bold text-gray-900">Description</h2>
          <p class="mt-4 whitespace-pre-line leading-relaxed text-gray-600">{{ item.description || 'No description provided.' }}</p>
        </div>
      </div>
    }
  `,
})
export class ListingDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ListingApiService);

  readonly listing = signal<PropertyListing | null>(null);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly categorySlug = signal('all');

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) return;

      this.loading.set(true);
      this.api.getListingById(id).subscribe({
        next: (item) => {
          this.listing.set(item);
          this.categorySlug.set(item.category.toLowerCase().replace(/\s+/g, '-'));
          this.loading.set(false);
          this.api.incrementViewCount(id).subscribe();
        },
        error: () => {
          this.error.set('Listing not found.');
          this.loading.set(false);
        },
      });
    });
  }
}
