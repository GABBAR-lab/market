import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { LocaleService, AppLanguage } from '../../core/services/locale.service';

@Component({
  selector: 'app-top-bar',
  imports: [RouterLink],
  template: `
    <div class="ll-top-bar">
      <div class="section-container flex flex-wrap items-center justify-between gap-2 py-1.5 text-xs">
        <div class="flex flex-wrap items-center gap-3">
          <a routerLink="/all-ads" class="hover:text-gold-400">{{ locale.label('allAds') }}</a>
          <a routerLink="/categories" class="hover:text-gold-400">Categories</a>
          <button type="button" class="hover:text-gold-400" (click)="setLang('si')">සිංහල</button>
          <button type="button" class="hover:text-gold-400" (click)="setLang('ta')">தமிழ்</button>
        </div>
        <div class="flex items-center gap-3">
          <a routerLink="/chat" class="hover:text-gold-400">{{ locale.label('chat') }}</a>
          @if (auth.isLoggedIn()) {
            <a routerLink="/profile" class="hover:text-gold-400">{{ displayName() }}</a>
            <button type="button" class="font-semibold hover:text-gold-400" (click)="logout()">Logout</button>
          } @else {
            <a routerLink="/login" class="font-semibold hover:text-gold-400">Login</a>
            <a routerLink="/register" class="hover:text-gold-400">Register</a>
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
