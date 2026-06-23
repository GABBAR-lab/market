import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-site-header',
  imports: [RouterLink],
  template: `
    <header class="sticky top-0 z-40 border-b border-gray-100/80 bg-white/95 shadow-sm backdrop-blur-md">
      <div class="section-container flex items-center justify-between gap-4 py-3 sm:py-4">
        <a routerLink="/" class="group flex items-center gap-3">
          <div
            class="flex h-12 w-12 items-center justify-center rounded-2xl bg-gradient-to-br from-maroon-800 via-maroon-900 to-maroon-950 shadow-lg ring-2 ring-gold-500/30 transition group-hover:shadow-xl sm:h-14 sm:w-14"
          >
            <svg class="h-7 w-7 text-gold-400 sm:h-8 sm:w-8" viewBox="0 0 48 48" fill="currentColor">
              <path d="M24 4L8 18h4v20h24V18h4L24 4zm0 6.5L32 18H16l8-7.5zM14 36V20h20v16H14z" opacity="0.9"/>
            </svg>
          </div>
          <div>
            <span class="block text-lg font-extrabold tracking-wider text-maroon-900 sm:text-xl">
              LIVING LANKA
            </span>
            <span class="hidden text-xs font-medium text-gold-600 sm:block">Premium Classifieds & Real Estate</span>
          </div>
        </a>

        <div class="hidden flex-1 max-w-md px-6 lg:block">
          <a routerLink="/search" class="input input-bordered flex w-full items-center gap-2 bg-base-100 text-sm text-gray-400">
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            Search properties, vehicles, electronics...
          </a>
        </div>

        <div class="flex items-center gap-2 sm:gap-3">
          <a
            routerLink="/post-ad"
            class="btn btn-sm rounded-full border-2 border-gold-500 bg-gradient-to-r from-maroon-800 to-maroon-950 px-4 text-white shadow-md hover:border-gold-400 hover:shadow-lg sm:btn-md sm:px-6"
          >
            <span class="hidden sm:inline">Post Your </span>Ad
          </a>
        </div>
      </div>
    </header>
  `,
})
export class SiteHeaderComponent {}
