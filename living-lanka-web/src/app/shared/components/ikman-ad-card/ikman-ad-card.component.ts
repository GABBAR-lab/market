import { Component, Input, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { PropertyListing } from '../../../core/models/marketplace.models';
import { FavoritesService } from '../../../core/services/favorites.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-ikman-ad-card',
  imports: [RouterLink, DecimalPipe],
  template: `
    <article class="ad-card-pro group">
      <a [routerLink]="['/listing', listing.id]" class="relative block overflow-hidden rounded-lg bg-gray-100">
        @if (listing.isFeatured) {
          <span class="badge-featured absolute left-0 top-0 z-10">Featured</span>
        }
        <button
          type="button"
          class="absolute right-2 top-2 z-10 flex h-8 w-8 items-center justify-center rounded-full bg-white/95 shadow-sm transition hover:scale-110"
          [class.text-red-500]="saved()"
          [class.text-gray-400]="!saved()"
          (click)="toggleSave($event)"
          [attr.aria-label]="saved() ? 'Remove from saved' : 'Save listing'"
        >
          <svg class="h-4 w-4" [attr.fill]="saved() ? 'currentColor' : 'none'" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
          </svg>
        </button>
        <img
          [src]="listing.imageUrl"
          [alt]="listing.title"
          class="aspect-[4/3] w-full object-cover transition group-hover:scale-105"
          loading="lazy"
        />
      </a>
      <a [routerLink]="['/listing', listing.id]" class="mt-2 block space-y-1 p-1">
        <h3 class="line-clamp-2 text-sm font-semibold text-gray-900 group-hover:text-maroon-800">
          {{ listing.title }}
        </h3>
        <p class="text-xs text-gray-500">
          @if (showMember) {
            <span class="mr-1 font-semibold uppercase text-teal-700">Member</span>
          }
          {{ listing.location }}
          @if (listing.category) {
            <span>, {{ listing.category }}</span>
          }
        </p>
        @if (listing.price > 0) {
          <p class="text-sm font-bold text-gray-900">Rs {{ listing.price | number:'1.0-0' }}</p>
        }
        @if (listing.postedAt) {
          <p class="text-xs text-gray-400">{{ listing.postedAt }}</p>
        }
      </a>
    </article>
  `,
})
export class IkmanAdCardComponent implements OnInit {
  @Input({ required: true }) listing!: PropertyListing;
  @Input() showMember = false;

  private readonly favorites = inject(FavoritesService);
  private readonly toast = inject(ToastService);
  readonly saved = signal(false);

  ngOnInit(): void {
    this.saved.set(this.favorites.isSaved(this.listing.id));
  }

  toggleSave(e: Event): void {
    e.preventDefault();
    e.stopPropagation();
    const added = this.favorites.toggle(this.listing.id);
    this.saved.set(added);
    this.toast.show(added ? 'Saved to your list' : 'Removed from saved', added ? 'success' : 'info');
  }
}
