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
    <article class="ad-card-olx group">
      <a [routerLink]="['/listing', listing.id]" class="relative block aspect-[4/3] overflow-hidden bg-gray-100">
        @if (listing.isFeatured) {
          <span class="badge-featured absolute left-2 top-2 z-10">Featured</span>
        }
        <button
          type="button"
          class="absolute right-2 top-2 z-10 flex h-8 w-8 items-center justify-center rounded-full bg-white shadow"
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
          class="h-full w-full object-cover transition duration-300 group-hover:scale-105"
          loading="lazy"
        />
      </a>
      <a [routerLink]="['/listing', listing.id]" class="flex flex-1 flex-col p-3">
        <p class="text-base font-bold text-gray-900">
          @if (listing.price > 0) {
            Rs {{ listing.price | number:'1.0-0' }}
          } @else {
            <span class="text-sm">Contact for price</span>
          }
        </p>
        <h3 class="mt-1 line-clamp-2 text-sm text-gray-700 group-hover:text-maroon-900">
          {{ listing.title }}
        </h3>
        <p class="mt-auto pt-2 text-xs text-gray-400">
          {{ listing.location }}
          @if (listing.postedAt) {
            <span> · {{ listing.postedAt }}</span>
          }
        </p>
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
    this.favorites.toggle(this.listing.id).subscribe({
      next: (added) => {
        this.saved.set(added);
        this.toast.show(added ? 'Saved to your list' : 'Removed from saved', added ? 'success' : 'info');
      },
      error: () => this.toast.error('Could not update saved list'),
    });
  }
}
