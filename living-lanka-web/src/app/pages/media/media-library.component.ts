import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MediaApiService, MediaAsset } from '../../core/services/media-api.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-media-library',
  imports: [RouterLink, DatePipe],
  template: `
    <div class="ll-page-hero py-8">
      <div class="section-container flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h1 class="text-2xl font-bold text-white sm:text-3xl">My Media</h1>
          <p class="text-sm text-white/70">All your uploaded photos — listing images & avatars</p>
        </div>
        <a routerLink="/post-ad" class="ll-btn-sell">+ Upload via Post Ad</a>
      </div>
    </div>

    <div class="section-container py-8 pb-safe-nav md:pb-12">
      <div class="mb-6 flex flex-wrap gap-2">
        <button type="button" class="btn btn-sm" [class.bg-maroon-800]="filter() === 'all'" [class.text-white]="filter() === 'all'" (click)="setFilter('all')">All</button>
        <button type="button" class="btn btn-sm btn-outline" [class.bg-maroon-800]="filter() === 'listings'" [class.text-white]="filter() === 'listings'" (click)="setFilter('listings')">Listing photos</button>
        <button type="button" class="btn btn-sm btn-outline" [class.bg-maroon-800]="filter() === 'avatars'" [class.text-white]="filter() === 'avatars'" (click)="setFilter('avatars')">Avatars</button>
      </div>

      @if (loading()) {
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (i of [1,2,3,4,5,6]; track i) {
            <div class="skeleton aspect-square rounded-lg"></div>
          }
        </div>
      } @else if (!filtered().length) {
        <div class="premium-card py-16 text-center">
          <p class="text-gray-500">No media uploaded yet.</p>
          <a routerLink="/post-ad" class="ll-btn-primary mt-4 inline-flex">Post an ad with photos</a>
        </div>
      } @else {
        <p class="mb-4 text-sm text-gray-600">{{ filtered().length }} file(s)</p>
        <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
          @for (item of filtered(); track item.id) {
            <article class="premium-card overflow-hidden">
              <div class="relative aspect-square bg-gray-100">
                <img [src]="item.url" [alt]="item.fileName" class="h-full w-full object-cover" loading="lazy" />
                <span class="absolute left-2 top-2 rounded bg-black/60 px-2 py-0.5 text-[10px] font-bold uppercase text-white">
                  {{ item.category }}
                </span>
              </div>
              <div class="p-3">
                <p class="truncate text-xs text-gray-500">{{ item.createdAt | date:'medium' }}</p>
                <p class="text-xs text-gray-400">{{ formatSize(item.sizeBytes) }}</p>
                <button type="button" class="mt-2 w-full text-xs font-semibold text-red-600 hover:underline" (click)="remove(item)">
                  Delete
                </button>
              </div>
            </article>
          }
        </div>
      }

      @if (auth.user() && isAdmin()) {
        <section class="mt-12">
          <h2 class="mb-4 text-lg font-bold text-gray-900">Admin — all platform media</h2>
          @if (adminMedia().length) {
            <div class="grid grid-cols-2 gap-4 sm:grid-cols-4 md:grid-cols-6">
              @for (item of adminMedia(); track item.id) {
                <div class="premium-card overflow-hidden">
                  <img [src]="item.url" [alt]="item.fileName" class="aspect-square w-full object-cover" />
                  <p class="truncate p-2 text-[10px] text-gray-500">{{ item.category }}</p>
                </div>
              }
            </div>
          }
        </section>
      }
    </div>
  `,
})
export class MediaLibraryComponent implements OnInit {
  private readonly api = inject(MediaApiService);
  readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly items = signal<MediaAsset[]>([]);
  readonly adminMedia = signal<MediaAsset[]>([]);
  readonly loading = signal(true);
  readonly filter = signal<'all' | 'listings' | 'avatars'>('all');

  readonly filtered = signal<MediaAsset[]>([]);

  ngOnInit(): void {
    this.load();
  }

  setFilter(f: 'all' | 'listings' | 'avatars'): void {
    this.filter.set(f);
    this.applyFilter();
  }

  remove(item: MediaAsset): void {
    if (!confirm('Delete this file permanently?')) return;
    this.api.deleteMedia(item.id).subscribe({
      next: () => {
        this.toast.success('File deleted');
        this.load();
      },
      error: () => this.toast.error('Could not delete file'),
    });
  }

  formatSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }

  isAdmin(): boolean {
    return false;
  }

  private load(): void {
    this.loading.set(true);
    this.api.getMyMedia().subscribe({
      next: (items) => {
        this.items.set(items);
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private applyFilter(): void {
    const f = this.filter();
    const all = this.items();
    if (f === 'all') {
      this.filtered.set(all);
    } else {
      this.filtered.set(all.filter((x) => x.category === f));
    }
  }
}
