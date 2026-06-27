import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';
import { ListingApiService } from '../../core/services/listing-api.service';
import { PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-all-ads',
  imports: [RouterLink, IkmanAdCardComponent],
  template: `
    <div class="border-b border-gray-200 bg-gray-50 py-8">
      <div class="section-container">
        <h1 class="text-2xl font-bold text-gray-900 sm:text-3xl">All Ads in Sri Lanka</h1>
        <p class="mt-1 text-gray-600">{{ totalCount() }} ads found</p>
      </div>
    </div>
    <div class="section-container py-10 pb-safe-nav md:pb-12">
      @if (loading()) {
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (i of [1,2,3,4,5,6,7,8]; track i) {
            <div class="skeleton aspect-[4/3] rounded-lg"></div>
          }
        </div>
      } @else if (listings().length) {
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (listing of listings(); track listing.id) {
            <app-ikman-ad-card [listing]="listing" />
          }
        </div>
        @if (totalPages() > 1) {
          <div class="mt-10 flex justify-center gap-2">
            <button type="button" class="btn btn-sm" [disabled]="page() <= 1" (click)="go(page() - 1)">Prev</button>
            <span class="flex items-center px-4 text-sm text-gray-600">Page {{ page() }} of {{ totalPages() }}</span>
            <button type="button" class="btn btn-sm" [disabled]="page() >= totalPages()" (click)="go(page() + 1)">Next</button>
          </div>
        }
      } @else {
        <p class="text-gray-500">No ads yet. <a routerLink="/post-ad" class="text-teal-700 underline">Post the first one!</a></p>
      }
    </div>
  `,
})
export class AllAdsComponent implements OnInit {
  private readonly api = inject(ListingApiService);

  readonly listings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly page = signal(1);
  readonly totalPages = signal(1);
  readonly totalCount = signal(0);

  ngOnInit(): void {
    this.load();
  }

  go(p: number): void {
    this.page.set(p);
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.api.searchListings({}, this.page(), 20).subscribe({
      next: (r) => {
        this.listings.set(r.items);
        this.totalPages.set(r.totalPages);
        this.totalCount.set(r.totalCount);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
