import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-top-bar',
  imports: [RouterLink],
  template: `
    <div class="bg-maroon-950 text-white">
      <div class="section-container flex flex-wrap items-center justify-between gap-2 py-2 text-xs sm:text-sm">
        <div class="flex flex-wrap items-center gap-3 sm:gap-5">
          <span class="font-medium text-gold-400">Sri Lanka's Premium Marketplace</span>
          <span class="hidden opacity-60 sm:inline">{{ today }}</span>
        </div>
        <div class="flex items-center gap-3 sm:gap-4">
          @if (auth.isLoggedIn()) {
            <a routerLink="/profile" class="font-medium transition hover:text-gold-400">
              {{ displayName() }}
            </a>
            <a routerLink="/my-listings" class="hidden opacity-80 transition hover:opacity-100 sm:inline">My Ads</a>
            <button type="button" class="font-medium text-gold-400 transition hover:text-gold-300" (click)="logout()">
              Logout
            </button>
          } @else {
            <a routerLink="/login" class="font-medium transition hover:text-gold-400">Login</a>
            <a routerLink="/register" class="font-medium text-gold-400 transition hover:text-gold-300">Register</a>
          }
          <span class="text-base" aria-hidden="true">🇱🇰</span>
        </div>
      </div>
    </div>
  `,
})
export class TopBarComponent {
  readonly auth = inject(AuthService);

  readonly today = new Date().toLocaleDateString('en-LK', {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
  });

  displayName(): string {
    const u = this.auth.user();
    if (u?.firstName) return `${u.firstName} ${u.lastName ?? ''}`.trim();
    return u?.email?.split('@')[0] ?? 'Account';
  }

  logout(): void {
    this.auth.logout().subscribe();
  }
}
