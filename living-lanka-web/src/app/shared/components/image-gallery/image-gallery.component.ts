import { Component, Input, signal } from '@angular/core';

@Component({
  selector: 'app-image-gallery',
  template: `
    <div class="gallery-root">
      <div class="gallery-main relative overflow-hidden rounded-2xl bg-gray-100 ring-1 ring-black/5">
        @if (images.length > 1) {
          <button
            type="button"
            class="gallery-nav gallery-nav-prev"
            (click)="prev()"
            aria-label="Previous image"
          >
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            type="button"
            class="gallery-nav gallery-nav-next"
            (click)="next()"
            aria-label="Next image"
          >
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </button>
          <span class="gallery-counter">{{ activeIndex() + 1 }} / {{ images.length }}</span>
        }
        <img
          [src]="images[activeIndex()]"
          [alt]="alt"
          class="gallery-main-img aspect-[4/3] w-full object-cover lg:aspect-auto lg:min-h-[480px]"
        />
      </div>

      @if (images.length > 1) {
        <div class="mt-3 flex gap-2 overflow-x-auto pb-1">
          @for (url of images; track url; let i = $index) {
            <button
              type="button"
              class="gallery-thumb shrink-0"
              [class.gallery-thumb-active]="i === activeIndex()"
              (click)="activeIndex.set(i)"
              [attr.aria-label]="'View image ' + (i + 1)"
            >
              <img [src]="url" [alt]="alt + ' thumbnail ' + (i + 1)" class="h-full w-full object-cover" loading="lazy" />
            </button>
          }
        </div>
      }
    </div>
  `,
})
export class ImageGalleryComponent {
  @Input({ required: true }) images: string[] = [];
  @Input() alt = 'Listing photo';

  readonly activeIndex = signal(0);

  prev(): void {
    const len = this.images.length;
    this.activeIndex.set((this.activeIndex() - 1 + len) % len);
  }

  next(): void {
    const len = this.images.length;
    this.activeIndex.set((this.activeIndex() + 1) % len);
  }
}
