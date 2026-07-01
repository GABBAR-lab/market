import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { FavoritesService } from '../../core/services/favorites.service';
import { SRI_LANKA_PROVINCES } from '../../core/data/sri-lanka-locations';

@Component({
  selector: 'app-site-header',
  imports: [RouterLink, FormsModule],
  template: `
    <header class="ll-header">
      <div class="section-container flex items-center gap-2 py-2.5 sm:gap-4 sm:py-3">
        <a routerLink="/" class="flex shrink-0 items-center">
          <img
            src="/images/living-lanka-logo.png"
            alt="Living Lanka"
            class="h-11 w-auto rounded-md object-contain sm:h-12"
          />
        </a>

        <form class="hidden min-w-0 flex-1 lg:flex" (submit)="onSearch($event)">
          <div class="flex w-full items-center gap-0 overflow-hidden rounded bg-white shadow-sm">
            <select
              class="hidden max-w-[140px] shrink-0 border-0 border-r border-gray-200 bg-gray-50 px-3 py-2.5 text-xs text-gray-700 focus:outline-none xl:block"
              [(ngModel)]="location"
              name="headerLocation"
            >
              <option value="">All Sri Lanka</option>
              @for (p of provinces; track p.name) {
                <option [value]="p.name">{{ p.name }}</option>
              }
            </select>
            <input
              type="search"
              [(ngModel)]="query"
              name="headerSearch"
              class="min-w-0 flex-1 border-0 px-4 py-2.5 text-sm focus:outline-none"
              placeholder="Find Cars, Mobile Phones and more..."
            />
            <button type="submit" class="ll-btn-primary shrink-0 rounded-none px-5 py-2.5">
              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </button>
          </div>
        </form>

        <div class="ml-auto flex items-center gap-1 sm:gap-2">
          <button
            type="button"
            class="rounded p-2 text-white/80 hover:text-gold-400 lg:hidden"
            (click)="mobileSearchOpen.set(!mobileSearchOpen())"
            aria-label="Search"
          >
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </button>
          <a routerLink="/saved" class="relative hidden rounded p-2 text-white/80 hover:text-gold-400 sm:flex" aria-label="Saved">
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
            </svg>
            @if (favorites.count() > 0) {
              <span class="absolute -right-0.5 -top-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-gold-500 text-[9px] font-bold text-maroon-950">
                {{ favorites.count() }}
              </span>
            }
          </a>
          @if (auth.isLoggedIn()) {
            <a routerLink="/my-listings" class="hidden text-sm font-semibold text-white hover:text-gold-400 md:inline">My Ads</a>
            <button type="button" class="text-sm font-semibold text-gold-400 hover:text-gold-300" (click)="logout()">Logout</button>
          } @else {
            <a routerLink="/login" class="text-sm font-semibold text-white hover:text-gold-400">Login</a>
          }
          <a routerLink="/post-ad" class="ll-btn-sell text-xs sm:text-sm">+ Sell</a>
        </div>
      </div>

      @if (mobileSearchOpen()) {
        <div class="border-t border-maroon-900 px-4 pb-3 lg:hidden">
          <form class="flex gap-2" (submit)="onSearch($event)">
            <input type="search" [(ngModel)]="query" name="mobileSearch" class="ll-search-input flex-1" placeholder="Search..." />
            <button type="submit" class="ll-btn-primary px-4">Go</button>
          </form>
        </div>
      }
    </header>

    <div class="border-b border-maroon-900/30 bg-maroon-900">
      <div class="section-container flex flex-wrap items-center justify-between gap-2 py-2 text-sm">
        <div class="flex items-center gap-4 text-white/80">
          <a routerLink="/saved" class="hover:text-gold-400 sm:hidden">Saved</a>
          <a routerLink="/my-listings" class="hover:text-gold-400">My Ads</a>
          <a routerLink="/my-media" class="hover:text-gold-400">My Media</a>
          <a routerLink="/chat" class="hidden hover:text-gold-400 sm:inline">Chat</a>
        </div>
        <div class="flex items-center gap-3">
          @if (auth.isLoggedIn()) {
            <a routerLink="/profile" class="font-medium text-white hover:text-gold-400">{{ displayName() }}</a>
            <span class="text-white/30">|</span>
            <button type="button" class="font-semibold text-gold-400 hover:text-gold-300" (click)="logout()">Logout</button>
          } @else {
            <a routerLink="/login" class="font-semibold text-white hover:text-gold-400">Login</a>
            <span class="text-white/30">|</span>
            <a routerLink="/register" class="font-medium text-white/90 hover:text-gold-400">Register</a>
          }
        </div>
      </div>
    </div>

    <div class="ll-trust-strip hidden sm:block">
      <div class="section-container flex flex-wrap items-center justify-center gap-6 py-2 text-xs font-medium">
        <span class="flex items-center gap-1.5">
          <svg class="h-4 w-4 text-gold-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
          </svg>
          Safe Trading Tips
        </span>
        <span class="flex items-center gap-1.5">
          <svg class="h-4 w-4 text-gold-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a2 2 0 01-2.828 0l-4.244-4.243a8 8 0 1111.314 0z" />
          </svg>
          All Sri Lanka
        </span>
        <span class="flex items-center gap-1.5">
          <svg class="h-4 w-4 text-gold-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 11.5V14m0-2.5v-6a1.5 1.5 0 113 0m-3 6a1.5 1.5 0 00-3 0v2a7.5 7.5 0 0015 0v-5a1.5 1.5 0 00-3 0m-6-3V11m0-5.5v-1a1.5 1.5 0 013 0v1m0 0V11m0-5.5a1.5 1.5 0 013 0v3m0 0V11" />
          </svg>
          Trusted Handshake Deals
        </span>
      </div>
    </div>
  `,
})
export class SiteHeaderComponent {
  private readonly router = inject(Router);
  readonly auth = inject(AuthService);
  readonly favorites = inject(FavoritesService);
  readonly mobileSearchOpen = signal(false);
  readonly provinces = SRI_LANKA_PROVINCES;
  query = '';
  location = '';

  onSearch(e: Event): void {
    e.preventDefault();
    this.mobileSearchOpen.set(false);
    this.router.navigate(['/search'], {
      queryParams: { query: this.query || undefined, province: this.location || undefined },
    });
  }

  displayName(): string {
    const u = this.auth.user();
    if (u?.firstName) return `${u.firstName} ${u.lastName ?? ''}`.trim();
    return u?.email?.split('@')[0] ?? 'My Account';
  }

  logout(): void {
    this.auth.logout().subscribe();
  }
}
