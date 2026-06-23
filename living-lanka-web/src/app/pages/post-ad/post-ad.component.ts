import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ListingApiService } from '../../core/services/listing-api.service';
import { AuthService } from '../../core/services/auth.service';
import { Category } from '../../core/models/marketplace.models';
import { LocationResponse } from '../../core/models/api.models';
import { slugify } from '../../core/utils/mappers';

@Component({
  selector: 'app-post-ad',
  imports: [RouterLink, FormsModule],
  template: `
    <div class="bg-gradient-to-b from-maroon-950 to-maroon-900 py-12 text-white">
      <div class="section-container">
        <h1 class="text-3xl font-bold">Post Your Free Ad</h1>
        <p class="mt-2 text-white/70">Reach thousands of buyers across Sri Lanka</p>
      </div>
    </div>

    <div class="section-container py-10">
      @if (error()) {
        <div class="alert alert-error mb-6 max-w-3xl">{{ error() }}</div>
      }
      @if (success()) {
        <div class="alert alert-success mb-6 max-w-3xl">{{ success() }}</div>
      }

      <form class="premium-card mx-auto max-w-3xl space-y-5 p-6 sm:p-10" (ngSubmit)="onSubmit()">
        <div class="form-control">
          <label class="label"><span class="label-text font-semibold">Ad Title *</span></label>
          <input type="text" [(ngModel)]="title" name="title" required class="input input-bordered w-full" placeholder="e.g. 3 Bedroom House in Colombo 05" />
        </div>

        <div class="grid grid-cols-1 gap-5 sm:grid-cols-2">
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">Category *</span></label>
            <select [(ngModel)]="categoryId" name="categoryId" required class="select select-bordered w-full">
              <option value="">Select category</option>
              @for (cat of categories(); track cat.id) {
                <option [value]="cat.id">{{ cat.title }}</option>
              }
            </select>
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">Condition *</span></label>
            <select [(ngModel)]="condition" name="condition" class="select select-bordered w-full">
              <option value="New">New</option>
              <option value="Used">Used</option>
              <option value="Refurbished">Refurbished</option>
            </select>
          </div>
        </div>

        <div class="grid grid-cols-1 gap-5 sm:grid-cols-3">
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">Price (LKR)</span></label>
            <input type="number" [(ngModel)]="price" name="price" class="input input-bordered w-full" placeholder="0" />
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">Price Type</span></label>
            <select [(ngModel)]="priceType" name="priceType" class="select select-bordered w-full">
              <option value="Fixed">Fixed</option>
              <option value="Negotiable">Negotiable</option>
              <option value="Free">Free</option>
            </select>
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">City</span></label>
            <select [(ngModel)]="city" name="city" class="select select-bordered w-full">
              <option value="">Select city</option>
              @for (loc of cities(); track loc.id) {
                <option [value]="loc.name">{{ loc.name }}</option>
              }
            </select>
          </div>
        </div>

        <div class="form-control">
          <label class="label"><span class="label-text font-semibold">Description *</span></label>
          <textarea [(ngModel)]="description" name="description" required class="textarea textarea-bordered h-36 w-full" placeholder="Describe your item in detail..."></textarea>
        </div>

        <div class="form-control">
          <label class="label"><span class="label-text font-semibold">Image URL</span></label>
          <input type="url" [(ngModel)]="imageUrl" name="imageUrl" class="input input-bordered w-full" placeholder="https://..." />
        </div>

        <div class="grid grid-cols-1 gap-5 sm:grid-cols-2">
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">Contact Phone</span></label>
            <input type="tel" [(ngModel)]="contactPhone" name="phone" class="input input-bordered w-full" />
          </div>
          <div class="form-control">
            <label class="label"><span class="label-text font-semibold">Contact Email</span></label>
            <input type="email" [(ngModel)]="contactEmail" name="email" class="input input-bordered w-full" />
          </div>
        </div>

        <div class="flex gap-4">
          <label class="label cursor-pointer gap-2">
            <input type="checkbox" [(ngModel)]="showPhone" name="showPhone" class="checkbox checkbox-sm" />
            <span class="label-text">Show phone</span>
          </label>
          <label class="label cursor-pointer gap-2">
            <input type="checkbox" [(ngModel)]="showEmail" name="showEmail" class="checkbox checkbox-sm" />
            <span class="label-text">Show email</span>
          </label>
        </div>

        <div class="rounded-xl bg-blue-50 p-4 text-sm text-blue-900">
          <strong>Tip:</strong> After registering, logout and login once if posting fails.
        </div>

        <div class="flex flex-col gap-3 pt-2 sm:flex-row">
          <button type="submit" class="btn flex-1 bg-gradient-to-r from-maroon-800 to-maroon-950 text-white" [disabled]="submitting()">
            @if (submitting()) { <span class="loading loading-spinner loading-sm"></span> }
            Publish Ad
          </button>
          <a routerLink="/" class="btn btn-ghost flex-1">Cancel</a>
        </div>
      </form>
    </div>
  `,
})
export class PostAdComponent implements OnInit {
  readonly auth = inject(AuthService);
  private readonly api = inject(ListingApiService);
  private readonly router = inject(Router);

  readonly categories = signal<Category[]>([]);
  readonly cities = signal<LocationResponse[]>([]);
  readonly error = signal('');
  readonly success = signal('');
  readonly submitting = signal(false);

  title = '';
  categoryId = '';
  description = '';
  price?: number;
  priceType = 'Fixed';
  condition = 'Used';
  city = '';
  imageUrl = '';
  contactPhone = '';
  contactEmail = '';
  showPhone = true;
  showEmail = false;

  ngOnInit(): void {
    this.contactEmail = this.auth.user()?.email ?? '';
    this.api.getCategories().subscribe((cats) => this.categories.set(cats));
    this.api.getLocationsByType('City').subscribe({
      next: (locs) => this.cities.set(locs),
      error: () => this.api.getLocations().subscribe((locs) => this.cities.set(locs)),
    });
  }

  onSubmit(): void {
    if (!this.title || !this.categoryId || !this.description) return;

    this.error.set('');
    this.success.set('');
    this.submitting.set(true);

    const slug = slugify(this.title) + '-' + Date.now().toString(36);

    this.api
      .createListing({
        categoryId: this.categoryId,
        title: this.title,
        slug,
        description: this.description,
        price: this.price,
        currency: 'LKR',
        priceType: this.priceType,
        condition: this.condition,
        city: this.city || undefined,
        country: 'Sri Lanka',
        contactPhone: this.contactPhone || undefined,
        contactEmail: this.contactEmail || undefined,
        showPhone: this.showPhone,
        showEmail: this.showEmail,
        images: this.imageUrl
          ? [{ url: this.imageUrl, sortOrder: 0, isPrimary: true, altText: this.title }]
          : undefined,
      })
      .subscribe({
        next: (listing) => {
          this.success.set('Ad created successfully! Redirecting...');
          setTimeout(() => this.router.navigate(['/listing', listing.id]), 1500);
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(err.error?.error ?? 'Failed to create listing. Logout and login again, then retry.');
          this.submitting.set(false);
        },
      });
  }
}
