import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { FavoritesService } from '../../core/services/favorites.service';
import { PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-saved',
  imports: [RouterLink, IkmanAdCardComponent],
  template: `
    <div class="page-hero">
      <div class="section-container">
        <h1 class="text-2xl font-bold text-gray-900 sm:text-3xl">Saved listings</h1>
        <p class="mt-1 text-gray-600">Ads you've bookmarked for later</p>
      </div>
    </div>

    <div class="section-container py-8 pb-safe-nav md:pb-12">
      @if (loading()) {
        <div class="grid grid-cols-2 gap-4 md:grid-cols-3 xl:grid-cols-4">
          @for (i of [1,2,3,4]; track i) {
            <div class="skeleton aspect-[4/3] rounded-xl"></div>
          }
        </div>
      } @else if (listings().length) {
        <div class="grid grid-cols-2 gap-4 md:grid-cols-3 xl:grid-cols-4">
          @for (listing of listings(); track listing.id) {
            <app-ikman-ad-card [listing]="listing" />
          }
        </div>
      } @else {
        <div class="premium-card mx-auto max-w-lg py-16 text-center">
          <div class="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-red-50">
            <svg class="h-7 w-7 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
            </svg>
          </div>
          <h2 class="text-lg font-bold text-gray-900">No saved ads yet</h2>
          <p class="mt-2 text-sm text-gray-500">Tap the heart on any listing to save it here.</p>
          <a routerLink="/all-ads" class="btn-brand mt-6 inline-flex">Browse ads</a>
        </div>
      }
    </div>
  `,
})
export class SavedComponent implements OnInit {
  private readonly api = inject(ListingApiService);
  private readonly favorites = inject(FavoritesService);

  readonly listings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    const ids = this.favorites.getAll();
    if (!ids.length) {
      this.loading.set(false);
      return;
    }

    forkJoin(
      ids.map((id) =>
        this.api.getListingById(id).pipe(catchError(() => of(null)))
      )
    ).subscribe((results) => {
      this.listings.set(results.filter((r): r is PropertyListing => r !== null));
      this.loading.set(false);
    });
  }
}
