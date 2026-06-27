import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LocaleService, AppLanguage } from '../../core/services/locale.service';

@Component({
  selector: 'app-top-bar',
  imports: [RouterLink],
  template: `
    <div class="bg-gray-900 text-white">
      <div class="section-container flex flex-wrap items-center justify-between gap-2 py-2 text-xs sm:text-sm">
        <div class="flex flex-wrap items-center gap-3 sm:gap-4">
          <a routerLink="/categories" class="font-semibold hover:text-amber-400">Categories</a>
          <a routerLink="/all-ads" class="font-semibold hover:text-amber-400">{{ locale.label('allAds') }}</a>
          <button type="button" class="hover:text-amber-400" (click)="setLang('si')">සිංහල</button>
          <button type="button" class="hover:text-amber-400" (click)="setLang('ta')">தமிழ்</button>
          <button type="button" class="hidden hover:text-amber-400 sm:inline" (click)="setLang('en')">English</button>
        </div>
        <div class="flex items-center gap-3 sm:gap-4">
          <a routerLink="/saved" class="hover:text-amber-400">Saved</a>
          <a routerLink="/chat" class="flex items-center gap-1 hover:text-amber-400">
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
            {{ locale.label('chat') }}
          </a>
          @if (auth.isLoggedIn()) {
            <a routerLink="/my-listings" class="hover:text-amber-400">My Ads</a>
            <a routerLink="/profile" class="hover:text-amber-400">{{ displayName() }}</a>
            <button type="button" class="hover:text-amber-400" (click)="logout()">Logout</button>
          } @else {
            <a routerLink="/login" class="font-semibold hover:text-amber-400">{{ locale.label('login') }}</a>
          }
        </div>
      </div>
    </div>
  `,
})
export class TopBarComponent {
  readonly auth = inject(AuthService);
  readonly locale = inject(LocaleService);

  setLang(lang: AppLanguage): void {
    this.locale.setLanguage(lang);
  }

  displayName(): string {
    const u = this.auth.user();
    if (u?.firstName) return `${u.firstName} ${u.lastName ?? ''}`.trim();
    return u?.email?.split('@')[0] ?? 'Account';
  }

  logout(): void {
    this.auth.logout().subscribe();
  }
}
