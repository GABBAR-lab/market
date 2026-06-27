import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { ProfileService } from '../../core/services/profile.service';
import { PublicSellerProfile, PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-seller',
  imports: [IkmanAdCardComponent, DatePipe],
  template: `
    <div class="ll-page-hero py-6">
      <div class="section-container">
        @if (loading()) {
          <div class="skeleton h-24 rounded-xl"></div>
        } @else if (seller(); as s) {
          <div class="flex items-center gap-4">
            <div class="flex h-16 w-16 items-center justify-center rounded-full bg-gold-500 text-xl font-bold text-maroon-950">
              {{ s.displayName.charAt(0) }}
            </div>
            <div>
              <h1 class="text-2xl font-bold text-white">{{ s.displayName }}</h1>
              <p class="text-sm text-white/70">Member since {{ s.memberSince | date:'MMM yyyy' }}</p>
              <p class="mt-1 text-sm text-gold-400">{{ totalCount() }} active ads</p>
            </div>
          </div>
        } @else {
          <h1 class="text-xl font-bold text-white">Seller</h1>
        }
      </div>
    </div>

    <div class="section-container py-8 pb-safe-nav md:pb-12">
      @if (listings().length) {
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (listing of listings(); track listing.id) {
            <app-ikman-ad-card [listing]="listing" />
          }
        </div>
      } @else if (!loading()) {
        <p class="text-center text-gray-500">No active ads from this seller.</p>
      }
    </div>
  `,
})
export class SellerComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ListingApiService);
  private readonly profiles = inject(ProfileService);

  readonly seller = signal<PublicSellerProfile | null>(null);
  readonly listings = signal<PropertyListing[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const userId = params.get('userId');
      if (!userId) return;

      this.loading.set(true);
      forkJoin({
        profile: this.profiles.getPublicSeller(userId).pipe(catchError(() => of(null))),
        ads: this.api.getSellerListings(userId, 1, 24),
      }).subscribe({
        next: ({ profile, ads }) => {
          this.seller.set(profile);
          this.listings.set(ads.items);
          this.totalCount.set(ads.totalCount);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
    });
  }
}
