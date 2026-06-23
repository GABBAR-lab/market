import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ListingApiService } from '../../core/services/listing-api.service';
import { PropertyListing } from '../../core/models/marketplace.models';

@Component({
  selector: 'app-my-listings',
  imports: [RouterLink, DecimalPipe],
  template: `
    <div class="section-container py-12">
      <div class="mb-8 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p class="text-sm font-semibold uppercase tracking-widest text-gold-600">Dashboard</p>
          <h1 class="text-3xl font-bold text-gray-900">My Listings</h1>
        </div>
        <a routerLink="/post-ad" class="btn bg-maroon-800 text-white">+ Post New Ad</a>
      </div>

      @if (loading()) {
        <div class="space-y-4">
          @for (i of [1,2,3]; track i) {
            <div class="skeleton h-24 rounded-2xl"></div>
          }
        </div>
      } @else if (error()) {
        <div class="alert alert-error">{{ error() }}</div>
      } @else if (listings().length === 0) {
        <div class="premium-card py-16 text-center">
          <p class="text-gray-500">You haven't posted any ads yet.</p>
          <a routerLink="/post-ad" class="btn mt-4 bg-maroon-800 text-white">Post Your First Ad</a>
        </div>
      } @else {
        <div class="space-y-4">
          @for (item of listings(); track item.id) {
            <article class="premium-card flex flex-col gap-4 p-4 sm:flex-row sm:items-center sm:p-6">
              <img [src]="item.imageUrl" [alt]="item.title" class="h-28 w-full rounded-xl object-cover sm:h-24 sm:w-36" />
              <div class="min-w-0 flex-1">
                <div class="flex flex-wrap items-center gap-2">
                  <h3 class="font-bold text-gray-900">{{ item.title }}</h3>
                  <span class="badge badge-sm" [class]="statusClass(item.status)">{{ item.status }}</span>
                </div>
                <p class="text-sm text-gray-500">{{ item.category }} &bull; {{ item.location }}</p>
                <p class="mt-1 font-semibold text-maroon-800">
                  Rs {{ item.price | number:'1.0-0' }} {{ item.currency }}
                </p>
              </div>
              <div class="flex flex-wrap gap-2">
                <a [routerLink]="['/listing', item.id]" class="btn btn-sm btn-outline">View</a>
                @if (item.status === 'Draft') {
                  <button type="button" class="btn btn-sm btn-success" (click)="submit(item.id)">Submit</button>
                }
                <button type="button" class="btn btn-sm btn-error btn-outline" (click)="remove(item.id)">Delete</button>
              </div>
            </article>
          }
        </div>
      }
    </div>
  `,
})
export class MyListingsComponent implements OnInit {
  private readonly api = inject(ListingApiService);

  readonly listings = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.api.getMyListings().subscribe({
      next: (items) => {
        this.listings.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load your listings.');
        this.loading.set(false);
      },
    });
  }

  submit(id: string): void {
    this.api.submitForReview(id).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Failed to submit listing for review.'),
    });
  }

  remove(id: string): void {
    if (!confirm('Delete this listing?')) return;
    this.api.deleteListing(id).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Failed to delete listing.'),
    });
  }

  statusClass(status?: string): string {
    switch (status) {
      case 'Published': return 'badge-success';
      case 'Draft': return 'badge-ghost';
      case 'PendingReview': return 'badge-warning';
      case 'Sold': return 'badge-info';
      default: return 'badge-ghost';
    }
  }
}
