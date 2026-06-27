import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ListingApiService } from '../../core/services/listing-api.service';
import { ImageUploadService } from '../../core/services/image-upload.service';
import { AuthService } from '../../core/services/auth.service';
import { Category } from '../../core/models/marketplace.models';
import { PaymentCalculationResponse } from '../../core/models/api.models';
import { slugify } from '../../core/utils/mappers';
import {
  LISTING_PURPOSES,
  ListingPurpose,
  SRI_LANKA_PROVINCES,
} from '../../core/data/sri-lanka-locations';
import { AD_DURATION_PRESETS, POST_AD_SUBCATEGORIES, PostAdSubCategory } from '../../core/data/post-ad-subcategories';
import {
  formatSlMobileDisplay,
  isValidSlMobile,
  validateAdTitle,
  validateCardNumber,
  validateCvv,
  validateDescription,
  validateImageCount,
} from '../../core/utils/post-ad.validators';

@Component({
  selector: 'app-post-ad',
  imports: [RouterLink, FormsModule, DecimalPipe],
  templateUrl: './post-ad.component.html',
})
export class PostAdComponent implements OnInit {
  readonly auth = inject(AuthService);
  private readonly api = inject(ListingApiService);
  private readonly images = inject(ImageUploadService);
  private readonly router = inject(Router);

  readonly purposes = LISTING_PURPOSES;
  readonly durationPresets = AD_DURATION_PRESETS;
  readonly provinces = SRI_LANKA_PROVINCES;

  readonly step = signal<1 | 2>(1);
  readonly categories = signal<Category[]>([]);
  readonly error = signal('');
  readonly fieldErrors = signal<Record<string, string>>({});
  readonly submitting = signal(false);
  readonly paymentQuote = signal<PaymentCalculationResponse | null>(null);
  readonly uploadedUrls = signal<string[]>([]);
  readonly imagePreviews = signal<string[]>([]);

  listingPurpose: ListingPurpose = 'Sale';
  subCategorySlug = '';
  durationDays = 30;
  customDuration = false;
  customDurationDays = 45;
  title = '';
  description = '';
  price?: number;
  priceType = 'Fixed';
  condition = 'Used';
  province = '';
  district = '';
  address = '';
  latitude?: number;
  longitude?: number;
  whatsappNumber = '';
  mobileNumber = '';
  contactEmail = '';
  pendingListingId = '';

  cardNumber = '';
  cardHolderName = '';
  expiryMonth = '';
  expiryYear = '';
  cvv = '';

  readonly subCategories = computed(() => POST_AD_SUBCATEGORIES[this.listingPurpose] ?? []);

  get availableDistricts() {
    return this.provinces.find((x) => x.name === this.province)?.districts ?? [];
  }

  ngOnInit(): void {
    this.contactEmail = this.auth.user()?.email ?? '';
    this.api.getCategories().subscribe((cats) => this.categories.set(cats));
  }

  onPurposeChange(): void {
    this.subCategorySlug = '';
    this.paymentQuote.set(null);
  }

  onSubCategoryChange(): void {
    this.refreshQuote();
  }

  onProvinceChange(): void {
    this.district = '';
  }

  onDurationChange(): void {
    if (!this.customDuration) {
      this.refreshQuote();
    }
  }

  effectiveDuration(): number {
    return this.customDuration ? this.customDurationDays : this.durationDays;
  }

  selectedSubCategory(): PostAdSubCategory | undefined {
    return this.subCategories().find((s) => s.slug === this.subCategorySlug);
  }

  resolveCategoryId(): string | undefined {
    const sub = this.selectedSubCategory();
    if (!sub) return undefined;
    return this.categories().find((c) => c.slug === sub.categorySlug)?.id;
  }

  onFilesSelected(e: Event): void {
    const input = e.target as HTMLInputElement;
    if (!input.files?.length) return;

    const files = Array.from(input.files);
    const err = validateImageCount(this.uploadedUrls().length + files.length);
    if (err) {
      this.error.set(err);
      return;
    }

    this.error.set('');
    this.submitting.set(true);
    this.images.uploadImages(files).subscribe({
      next: (urls) => {
        this.uploadedUrls.update((prev) => [...prev, ...urls]);
        this.imagePreviews.update((prev) => [...prev, ...urls]);
        this.submitting.set(false);
        input.value = '';
      },
      error: () => {
        this.error.set('Failed to upload images. Try again.');
        this.submitting.set(false);
      },
    });
  }

  removeImage(index: number): void {
    this.uploadedUrls.update((prev) => prev.filter((_, i) => i !== index));
    this.imagePreviews.update((prev) => prev.filter((_, i) => i !== index));
  }

  useMyLocation(): void {
    if (!navigator.geolocation) {
      this.error.set('Geolocation is not supported by your browser.');
      return;
    }
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        this.latitude = pos.coords.latitude;
        this.longitude = pos.coords.longitude;
      },
      () => this.error.set('Could not get your location.')
    );
  }

  validateForm(): boolean {
    const errors: Record<string, string> = {};

    if (!this.listingPurpose) errors['purpose'] = 'Main category is required.';
    if (!this.subCategorySlug) errors['subCategory'] = 'Sub category is required.';

    const titleErr = validateAdTitle(this.title);
    if (titleErr) errors['title'] = titleErr;

    const descErr = validateDescription(this.description);
    if (descErr) errors['description'] = descErr;

    if (!this.province) errors['province'] = 'Province is required.';
    if (!this.district) errors['district'] = 'District is required.';

    if (!isValidSlMobile(this.whatsappNumber)) errors['whatsapp'] = 'Valid 10-digit Sri Lankan WhatsApp number required.';
    if (!isValidSlMobile(this.mobileNumber)) errors['mobile'] = 'Valid 10-digit Sri Lankan mobile number required.';

    const dur = this.effectiveDuration();
    if (dur < 1 || dur > 365) errors['duration'] = 'Duration must be 1–365 days.';

    const imgErr = validateImageCount(this.uploadedUrls().length);
    if (imgErr) errors['images'] = imgErr;

    if (!this.resolveCategoryId()) errors['subCategory'] = 'Could not map sub category. Refresh and try again.';

    this.fieldErrors.set(errors);
    return Object.keys(errors).length === 0;
  }

  goToPayment(): void {
    this.error.set('');
    if (!this.validateForm()) {
      this.error.set('Please fix the errors below.');
      return;
    }

    const categoryId = this.resolveCategoryId()!;
    this.submitting.set(true);

    this.api
      .calculatePayment({
        categoryId,
        listingPurpose: this.listingPurpose,
        durationDays: this.effectiveDuration(),
      })
      .subscribe({
        next: (quote) => {
          this.paymentQuote.set(quote);
          this.createDraftListing(categoryId, quote.totalAmount);
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(err.error?.error ?? 'Could not calculate payment.');
          this.submitting.set(false);
        },
      });
  }

  private createDraftListing(categoryId: string, _total: number): void {
    const slug = slugify(this.title) + '-' + Date.now().toString(36);
    const dur = this.effectiveDuration();
    const sub = this.selectedSubCategory()!;

    this.api
      .createListing({
        categoryId,
        title: this.title.trim(),
        slug,
        description: this.buildDescription(sub.label),
        price: this.price,
        currency: 'LKR',
        priceType: this.priceType,
        condition: this.condition,
        listingPurpose: this.listingPurpose,
        mobilePhone: formatSlMobileDisplay(this.mobileNumber),
        whatsAppPhone: formatSlMobileDisplay(this.whatsappNumber),
        address: this.address.trim() || undefined,
        adDurationDays: dur,
        province: this.province,
        district: this.district,
        city: this.district,
        country: 'Sri Lanka',
        contactPhone: formatSlMobileDisplay(this.mobileNumber),
        contactEmail: this.contactEmail || undefined,
        showPhone: true,
        showEmail: !!this.contactEmail,
        latitude: this.latitude,
        longitude: this.longitude,
        images: this.uploadedUrls().map((url, i) => ({
          url,
          sortOrder: i,
          isPrimary: i === 0,
          altText: this.title,
        })),
      })
      .subscribe({
        next: (listing) => {
          this.pendingListingId = listing.id;
          this.step.set(2);
          this.submitting.set(false);
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(err.error?.error ?? 'Failed to save advertisement.');
          this.submitting.set(false);
        },
      });
  }

  payAndPublish(): void {
    this.error.set('');
    const errors: Record<string, string> = {};
    const cardErr = validateCardNumber(this.cardNumber);
    if (cardErr) errors['cardNumber'] = cardErr;
    if (!this.cardHolderName.trim()) errors['cardHolderName'] = 'Card holder name is required.';
    if (!this.expiryMonth || !this.expiryYear) errors['expiry'] = 'Expiry date is required.';
    const cvvErr = validateCvv(this.cvv);
    if (cvvErr) errors['cvv'] = cvvErr;
    this.fieldErrors.set(errors);
    if (Object.keys(errors).length) {
      this.error.set('Please fix payment details.');
      return;
    }

    this.submitting.set(true);
    this.api
      .completePayment({
        listingId: this.pendingListingId,
        cardNumber: this.cardNumber.replace(/\D/g, ''),
        cardHolderName: this.cardHolderName.trim(),
        expiryMonth: this.expiryMonth,
        expiryYear: this.expiryYear,
        cvv: this.cvv,
      })
      .subscribe({
        next: () => {
          this.router.navigate(['/listing', this.pendingListingId]);
        },
        error: (err: HttpErrorResponse) => {
          this.error.set(err.error?.error ?? 'Payment failed. Your ad remains pending payment.');
          this.submitting.set(false);
        },
      });
  }

  backToForm(): void {
    this.step.set(1);
    this.error.set('');
  }

  private buildDescription(subLabel: string): string {
    return [
      `Sub Category: ${subLabel}`,
      `Listing Type: For ${this.listingPurpose}`,
      `Duration: ${this.effectiveDuration()} days`,
      this.address ? `Address: ${this.address}` : null,
      `Location: ${this.district}, ${this.province}, Sri Lanka`,
      this.latitude && this.longitude ? `GPS: ${this.latitude}, ${this.longitude}` : null,
      '',
      this.description.trim(),
    ]
      .filter(Boolean)
      .join('\n');
  }

  private refreshQuote(): void {
    const categoryId = this.resolveCategoryId();
    if (!categoryId) return;
    this.api
      .calculatePayment({
        categoryId,
        listingPurpose: this.listingPurpose,
        durationDays: this.effectiveDuration(),
      })
      .subscribe({
        next: (q) => this.paymentQuote.set(q),
        error: () => this.paymentQuote.set(null),
      });
  }
}
