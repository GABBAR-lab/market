import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DecimalPipe, DatePipe } from '@angular/common';
import { ListingApiService } from '../../core/services/listing-api.service';
import { ToastService } from '../../core/services/toast.service';
import { PropertyListing } from '../../core/models/marketplace.models';

interface SellerInquiry {
  id: string;
  listingId: string;
  listingTitle: string;
  buyerName: string;
  buyerPhone: string;
  message: string;
  status: string;
  createdAt: string;
}

@Component({
  selector: 'app-my-listings',
  imports: [RouterLink, DecimalPipe, DatePipe],
  template: `
    <div class="ll-page-hero py-8">
      <div class="section-container flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h1 class="text-2xl font-bold text-white sm:text-3xl">My Ads</h1>
          <p class="text-sm text-white/70">Manage listings and buyer messages</p>
        </div>
        <a routerLink="/post-ad" class="ll-btn-sell">+ Post New Ad</a>
      </div>
    </div>

    <div class="section-container py-8 pb-safe-nav md:pb-12">
      @if (inquiries().length) {
        <section class="mb-10">
          <h2 class="mb-4 text-lg font-bold text-gray-900">Buyer messages ({{ inquiries().length }})</h2>
          <div class="space-y-3">
            @for (inq of inquiries(); track inq.id) {
              <article class="premium-card p-4">
                <div class="flex flex-wrap items-start justify-between gap-2">
                  <div>
                    <p class="font-semibold text-gray-900">{{ inq.buyerName }} · {{ inq.buyerPhone }}</p>
                    <p class="text-xs text-gray-500">Re: {{ inq.listingTitle }} · {{ inq.createdAt | date:'medium' }}</p>
                  </div>
                  <a [routerLink]="['/listing', inq.listingId]" class="text-sm font-semibold text-maroon-800 hover:underline">View ad</a>
                </div>
                <p class="mt-2 text-sm text-gray-600">{{ inq.message }}</p>
              </article>
            }
          </div>
        </section>
      }

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
                  <span class="badge badge-sm" [class]="statusClass(item.status)">{{ statusLabel(item.status) }}</span>
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
                @if (canMarkSold(item.status)) {
                  <button type="button" class="btn btn-sm btn-info" (click)="markSold(item.id)">Mark as sold</button>
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
  private readonly toast = inject(ToastService);

  readonly listings = signal<PropertyListing[]>([]);
  readonly inquiries = signal<SellerInquiry[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');

  ngOnInit(): void {
    this.load();
    this.api.getMyInquiries().subscribe({
      next: (items) => this.inquiries.set(items),
      error: () => {},
    });
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

  markSold(id: string): void {
    if (!confirm('Mark this ad as sold?')) return;
    this.api.markAsSold(id).subscribe({
      next: () => {
        this.toast.success('Ad marked as sold');
        this.load();
      },
      error: () => this.error.set('Failed to mark listing as sold.'),
    });
  }

  canMarkSold(status?: string): boolean {
    return status === 'Active' || status === 'Published' || status === 'PendingReview';
  }

  statusClass(status?: string): string {
    switch (status) {
      case 'Active':
      case 'Published': return 'badge-success';
      case 'Draft': return 'badge-ghost';
      case 'PendingReview': return 'badge-warning';
      case 'PendingPayment': return 'badge-warning';
      case 'Sold': return 'badge-info';
      case 'Expired': return 'badge-ghost';
      case 'Rejected': return 'badge-error';
      default: return 'badge-ghost';
    }
  }

  statusLabel(status?: string): string {
    if (!status) return 'Unknown';
    if (status === 'PendingPayment') return 'Awaiting payment';
    if (status === 'PendingReview') return 'Under review';
    return status;
  }
}
