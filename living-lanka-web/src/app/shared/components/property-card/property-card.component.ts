import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PropertyListing } from '../../../core/models/marketplace.models';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-property-card',
  imports: [RouterLink, DecimalPipe],
  template: `
    <article class="card group overflow-hidden bg-white shadow-lg ring-1 ring-black/5 transition hover:-translate-y-1 hover:shadow-2xl">
      <figure class="relative overflow-hidden">
        <img [src]="listing.imageUrl" [alt]="listing.title" class="h-44 w-full object-cover transition duration-500 group-hover:scale-110 sm:h-48" loading="lazy" />
        <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent"></div>
        @if (listing.badge) {
          <span
            class="badge absolute left-3 top-3 border-0 text-white shadow-lg"
            [class]="listing.badge === 'hot' ? 'bg-red-500' : 'bg-maroon-800'"
          >
            {{ listing.badge === 'hot' ? 'Hot Offer' : 'New' }}
          </span>
        }
        @if (listing.priceType) {
          <span class="absolute bottom-3 right-3 rounded-full bg-black/60 px-2 py-0.5 text-xs text-white">{{ listing.priceType }}</span>
        }
      </figure>
      <div class="card-body items-center p-4 text-center sm:p-5">
        <p class="text-lg font-bold text-maroon-800">
          @if (listing.price > 0) {
            Rs {{ listing.price | number:'1.0-0' }} {{ listing.currency }}
          } @else {
            Contact for Price
          }
        </p>
        <h3 class="line-clamp-2 font-bold text-gray-800">{{ listing.title }}</h3>
        <p class="text-sm text-gray-400">{{ listing.location }}</p>
        @if (listing.bedrooms || listing.areaSqFt) {
          <p class="text-xs text-gray-500">
            @if (listing.bedrooms) { {{ listing.bedrooms }} Bed. }
            @if (listing.areaSqFt) { &bull; {{ listing.areaSqFt }} Sq.Ft }
            @if (listing.bathrooms) { &bull; {{ listing.bathrooms }} Bath }
          </p>
        }
        <a [routerLink]="['/listing', listing.id]" class="btn btn-success btn-block mt-2">View Details</a>
      </div>
    </article>
  `,
})
export class PropertyCardComponent {
  @Input({ required: true }) listing!: PropertyListing;
}

@Component({
  selector: 'app-listing-card',
  imports: [RouterLink],
  template: `
    <article class="card group overflow-hidden border border-gray-100 bg-white shadow-sm transition hover:shadow-xl">
      <figure class="relative overflow-hidden">
        <img [src]="listing.imageUrl" [alt]="listing.title" class="h-40 w-full object-cover transition duration-500 group-hover:scale-105" loading="lazy" />
        @if (listing.badge) {
          <span
            class="badge absolute right-2 top-2 border-0 text-white"
            [class]="listing.badge === 'hot' ? 'bg-red-500' : 'bg-maroon-800'"
          >
            {{ listing.badge === 'hot' ? 'Hot Offer' : 'New' }}
          </span>
        }
        <div class="absolute inset-x-0 bottom-0 flex items-center gap-3 bg-gradient-to-t from-black/80 to-transparent px-3 py-3 text-xs text-white">
          <span>{{ listing.postedAt ?? 'Recently' }}</span>
          @if (listing.viewCount) {
            <span>{{ listing.viewCount }} views</span>
          }
        </div>
      </figure>
      <div class="card-body p-4">
        <span class="text-xs font-semibold uppercase tracking-wider text-gold-600">{{ listing.category }}</span>
        <h3 class="line-clamp-2 font-bold text-gray-900">{{ listing.title }}</h3>
        <a [routerLink]="['/listing', listing.id]" class="btn btn-ghost btn-sm mt-2 px-0 text-maroon-800">
          View Details &rarr;
        </a>
      </div>
    </article>
  `,
})
export class ListingCardComponent {
  @Input({ required: true }) listing!: PropertyListing;
}
