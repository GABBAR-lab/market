import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DecimalPipe, TitleCasePipe } from '@angular/common';
import { ListingApiService } from '../../core/services/listing-api.service';
import { FavoritesService } from '../../core/services/favorites.service';
import { ToastService } from '../../core/services/toast.service';
import { PropertyListing } from '../../core/models/marketplace.models';
import { ImageGalleryComponent } from '../../shared/components/image-gallery/image-gallery.component';
import { IkmanAdCardComponent } from '../../shared/components/ikman-ad-card/ikman-ad-card.component';

@Component({
  selector: 'app-listing-detail',
  imports: [RouterLink, DecimalPipe, TitleCasePipe, ImageGalleryComponent, IkmanAdCardComponent],
  template: `
    @if (loading()) {
      <div class="section-container py-12">
        <div class="skeleton mb-4 h-6 w-64 rounded-lg"></div>
        <div class="grid gap-8 lg:grid-cols-5">
          <div class="skeleton aspect-[4/3] rounded-2xl lg:col-span-3"></div>
          <div class="skeleton h-80 rounded-2xl lg:col-span-2"></div>
        </div>
      </div>
    } @else if (error()) {
      <div class="section-container py-24 text-center">
        <div class="mx-auto max-w-md">
          <div class="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-gray-100">
            <svg class="h-8 w-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h1 class="text-xl font-bold text-gray-900">Listing not found</h1>
          <p class="mt-2 text-gray-500">{{ error() }}</p>
          <a routerLink="/" class="btn mt-4 bg-maroon-800 text-white">Back Home</a>
        </div>
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

      <div class="section-container -mt-4 py-10 pb-safe-nav md:pb-12">
        <div class="grid grid-cols-1 gap-8 lg:grid-cols-5">
          <div class="lg:col-span-3">
            <app-image-gallery [images]="galleryImages()" [alt]="item.title" />
          </div>

          <div class="lg:col-span-2">
            <div class="premium-card sticky top-24 p-6 sm:p-7">
              <div class="flex flex-wrap items-start justify-between gap-3">
                <div class="flex flex-wrap gap-2">
                  @if (item.isFeatured) {
                    <span class="badge-featured">Featured</span>
                  }
                  <span class="rounded-md bg-gray-100 px-2 py-0.5 text-xs font-medium text-gray-700">{{ item.condition }}</span>
                </div>
                <div class="flex gap-2">
                  <button type="button" class="rounded-xl border border-gray-200 p-2.5 transition hover:bg-gray-50" (click)="toggleSave()" [attr.aria-label]="isSaved() ? 'Unsave' : 'Save'">
                    <svg class="h-5 w-5" [class.text-red-500]="isSaved()" [class.text-gray-500]="!isSaved()" [attr.fill]="isSaved() ? 'currentColor' : 'none'" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                    </svg>
                  </button>
                  <button type="button" class="rounded-xl border border-gray-200 p-2.5 transition hover:bg-gray-50" (click)="shareListing()" aria-label="Share listing">
                    <svg class="h-5 w-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
                    </svg>
                  </button>
                </div>
              </div>

              <h1 class="mt-4 text-2xl font-bold leading-tight text-gray-900 sm:text-3xl">{{ item.title }}</h1>

              <p class="mt-4 text-3xl font-extrabold text-maroon-800">
                @if (item.price > 0) {
                  Rs {{ item.price | number:'1.0-0' }}
                } @else {
                  <span class="text-xl">Contact for price</span>
                }
              </p>
              <p class="text-sm text-gray-500">{{ item.priceType | titlecase }} · {{ item.currency }}</p>

              <p class="mt-4 flex items-start gap-2 text-sm text-gray-600">
                <svg class="mt-0.5 h-4 w-4 shrink-0 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.828 0l-4.244-4.243a8 8 0 1111.314 0z" />
                </svg>
                {{ item.location }}
              </p>

              @if (item.viewCount) {
                <p class="mt-2 text-xs text-gray-400">{{ item.viewCount }} people viewed this ad</p>
              }

              <div class="mt-6 space-y-2">
                @if (item.showPhone && item.contactPhone) {
                  <a [href]="whatsappLink(item.contactPhone)" target="_blank" rel="noopener noreferrer" class="btn btn-success btn-block gap-2 rounded-xl">
                    <svg class="h-5 w-5" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347z"/>
                      <path d="M12 2C6.477 2 2 6.477 2 12c0 1.89.525 3.66 1.438 5.168L2 22l4.832-1.438A9.955 9.955 0 0012 22c5.523 0 10-4.477 10-10S17.523 2 12 2z"/>
                    </svg>
                    Chat on WhatsApp
                  </a>
                  <a [href]="'tel:' + item.contactPhone" class="btn btn-outline btn-block rounded-xl">Call {{ item.contactPhone }}</a>
                }
                @if (item.showEmail && item.contactEmail) {
                  <a [href]="'mailto:' + item.contactEmail" class="btn btn-outline btn-block rounded-xl">Email Seller</a>
                }
              </div>

              <div class="mt-6 rounded-xl bg-amber-50 p-4 text-xs leading-relaxed text-amber-900">
                <strong class="font-semibold">Safety tip:</strong> Meet in a public place, inspect items before paying, and never share OTP or bank PINs.
              </div>
            </div>
          </div>
        </div>

        <div class="premium-card mt-8 p-6 sm:p-8">
          <h2 class="text-lg font-bold text-gray-900">Description</h2>
          <p class="mt-4 whitespace-pre-line leading-relaxed text-gray-600">{{ item.description || 'No description provided.' }}</p>
        </div>

        @if (related().length) {
          <section class="mt-12">
            <h2 class="text-xl font-bold text-gray-900">Similar listings</h2>
            <div class="mt-6 grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
              @for (rel of related(); track rel.id) {
                <app-ikman-ad-card [listing]="rel" />
              }
            </div>
          </section>
        }
      </div>
    }
  `,
})
export class ListingDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ListingApiService);
  private readonly favorites = inject(FavoritesService);
  private readonly toast = inject(ToastService);

  readonly listing = signal<PropertyListing | null>(null);
  readonly related = signal<PropertyListing[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly categorySlug = signal('all');
  readonly isSaved = signal(false);

  readonly galleryImages = computed(() => {
    const item = this.listing();
    if (!item) return [];
    return item.imageUrls?.length ? item.imageUrls : [item.imageUrl];
  });

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) return;

      this.loading.set(true);
      this.api.getListingById(id).subscribe({
        next: (item) => {
          this.listing.set(item);
          this.categorySlug.set(item.category.toLowerCase().replace(/\s+/g, '-'));
          this.isSaved.set(this.favorites.isSaved(item.id));
          this.loading.set(false);
          this.updatePageTitle(item.title);
          this.api.incrementViewCount(id).subscribe();
          this.loadRelated(item);
        },
        error: () => {
          this.error.set('This listing may have been removed or expired.');
          this.loading.set(false);
        },
      });
    });
  }

  toggleSave(): void {
    const item = this.listing();
    if (!item) return;
    const added = this.favorites.toggle(item.id);
    this.isSaved.set(added);
    this.toast.show(added ? 'Saved to your list' : 'Removed from saved', added ? 'success' : 'info');
  }

  shareListing(): void {
    const item = this.listing();
    if (!item) return;
    const url = window.location.href;
    const text = `${item.title} — Rs ${item.price.toLocaleString()}`;
    if (navigator.share) {
      navigator.share({ title: item.title, text, url }).catch(() => this.copyLink(url));
    } else {
      this.copyLink(url);
    }
  }

  whatsappLink(phone: string): string {
    const digits = phone.replace(/\D/g, '');
    const msg = encodeURIComponent(`Hi, I'm interested in: ${this.listing()?.title ?? 'your ad'}`);
    return `https://wa.me/${digits}?text=${msg}`;
  }

  private loadRelated(item: PropertyListing): void {
    this.api.searchListings({ categoryId: item.categoryId }, 1, 6).subscribe({
      next: (result) => {
        this.related.set(result.items.filter((l) => l.id !== item.id).slice(0, 5));
      },
    });
  }

  private copyLink(url: string): void {
    navigator.clipboard.writeText(url).then(() => this.toast.success('Link copied to clipboard'));
  }

  private updatePageTitle(title: string): void {
    document.title = `${title} | Living Lanka`;
  }
}
