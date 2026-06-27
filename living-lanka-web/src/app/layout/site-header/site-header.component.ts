import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LocaleService } from '../../core/services/locale.service';
import { FavoritesService } from '../../core/services/favorites.service';

@Component({
  selector: 'app-site-header',
  imports: [RouterLink, FormsModule],
  template: `
    <header class="sticky top-0 z-40 border-b border-gray-200 bg-white shadow-sm">
      <div class="section-container flex items-center gap-4 py-3">
        <a routerLink="/" class="flex shrink-0 items-center gap-2">
          <img
            src="/images/living-lanka-logo.png"
            alt="Living Lanka"
            class="h-10 w-auto object-contain sm:h-12"
          />
        </a>

        <form class="hidden flex-1 md:flex" (submit)="onSearch($event)">
          <input
            type="search"
            [(ngModel)]="query"
            name="headerSearch"
            class="input input-bordered w-full rounded-r-none border-r-0"
            placeholder="Search in Living Lanka..."
          />
          <button type="submit" class="btn rounded-l-none bg-teal-600 text-white hover:bg-teal-700">
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </button>
        </form>

        <a
          routerLink="/saved"
          class="relative hidden rounded-lg p-2 text-gray-600 hover:bg-gray-100 sm:flex"
          aria-label="Saved"
        >
          <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
          </svg>
          @if (favorites.count() > 0) {
            <span class="absolute right-0 top-0 flex h-4 min-w-4 items-center justify-center rounded-full bg-maroon-700 px-1 text-[9px] font-bold text-white">
              {{ favorites.count() }}
            </span>
          }
        </a>

        <a
          routerLink="/post-ad"
          class="btn shrink-0 border-0 bg-amber-400 px-4 text-xs font-bold uppercase text-gray-900 hover:bg-amber-300 sm:px-6 sm:text-sm"
        >
          {{ locale.label('postAd') }}
        </a>
      </div>

      @if (mobileSearchOpen()) {
        <form class="border-t border-gray-100 px-4 py-3 md:hidden" (submit)="onSearch($event)">
          <div class="flex gap-2">
            <input type="search" [(ngModel)]="query" name="mobileSearch" class="input input-bordered flex-1" placeholder="Search..." />
            <button type="submit" class="btn bg-teal-600 text-white">Go</button>
          </div>
        </form>
      }
    </header>
  `,
})
export class SiteHeaderComponent {
  private readonly router = inject(Router);
  readonly locale = inject(LocaleService);
  readonly favorites = inject(FavoritesService);
  readonly mobileSearchOpen = signal(false);
  query = '';

  onSearch(e: Event): void {
    e.preventDefault();
    this.mobileSearchOpen.set(false);
    this.router.navigate(['/search'], { queryParams: { query: this.query || undefined } });
  }
}
