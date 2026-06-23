import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-site-footer',
  imports: [RouterLink],
  template: `
    <footer class="bg-maroon-900 text-white">
      <div class="section-container grid grid-cols-1 gap-10 py-12 md:grid-cols-2 lg:grid-cols-3">
        <div>
          <h3 class="mb-4 text-lg font-bold text-gold-400">Information</h3>
          <ul class="space-y-2 text-sm text-white/80">
            @for (link of infoLinks; track link.path) {
              <li>
                <a [routerLink]="link.path" class="transition hover:text-gold-400">{{ link.label }}</a>
              </li>
            }
          </ul>
        </div>

        <div>
          <h3 class="mb-4 text-lg font-bold text-gold-400">Our Reach Across Sri Lanka</h3>
          <ul class="grid grid-cols-2 gap-2 text-sm text-white/80">
            @for (city of cities; track city) {
              <li class="flex items-center gap-1.5">
                <svg class="h-3.5 w-3.5 shrink-0 text-gold-400" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clip-rule="evenodd" />
                </svg>
                {{ city }}
              </li>
            }
          </ul>
        </div>

        <div class="flex items-center justify-center md:justify-end">
          <svg class="h-48 w-48 opacity-20 lg:h-56 lg:w-56" viewBox="0 0 100 180" fill="currentColor">
            <path d="M52 5c-8 12-18 28-20 45-2 18 4 35 12 48 8 14 18 28 16 42-2 14-12 28-20 38-4 5-8 2-6-4 4-10 10-24 8-38-2-16-14-30-20-42C18 78 12 58 18 40c4-12 14-24 22-32 4-4 8-6 12-3z" />
          </svg>
        </div>
      </div>

      <div class="border-t border-white/10">
        <div class="section-container flex flex-col items-center justify-between gap-4 py-6 text-sm text-white/60 sm:flex-row">
          <p>&copy; {{ year }} Living Lanka. All rights reserved.</p>
          <div class="flex gap-4">
            <a routerLink="/privacy" class="hover:text-gold-400">Privacy</a>
            <a routerLink="/terms" class="hover:text-gold-400">Terms</a>
          </div>
        </div>
      </div>
    </footer>
  `,
})
export class SiteFooterComponent {
  readonly year = new Date().getFullYear();

  readonly infoLinks = [
    { label: 'Home', path: '/' },
    { label: 'About Us', path: '/about' },
    { label: 'Contact Us', path: '/contact' },
    { label: 'Terms & Conditions', path: '/terms' },
    { label: 'Privacy Policy', path: '/privacy' },
    { label: 'FAQ', path: '/faq' },
  ];

  readonly cities = [
    'Colombo', 'Kandy', 'Galle', 'Jaffna',
    'Negombo', 'Matara', 'Anuradhapura', 'Trincomalee',
  ];
}
