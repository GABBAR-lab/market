import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-site-footer',
  imports: [RouterLink],
  template: `
    <footer class="ll-footer">
      <div class="section-container grid grid-cols-1 gap-10 py-12 sm:grid-cols-2 lg:grid-cols-4">
        <div>
          <img src="/images/living-lanka-logo.png" alt="Living Lanka" class="mb-4 h-14 w-auto rounded-md" />
          <p class="text-sm text-white/70">Sri Lanka's trusted classified marketplace. Buy, sell & rent across the island.</p>
        </div>
        <div>
          <h3 class="mb-4 font-bold text-gold-400">Sell on Living Lanka</h3>
          <ul class="space-y-2 text-sm text-white/80">
            <li><a routerLink="/post-ad" class="hover:text-gold-400">Post Free Ad</a></li>
            <li><a routerLink="/sell-fast" class="hover:text-gold-400">Sell Fast</a></li>
            <li><a routerLink="/membership" class="hover:text-gold-400">Membership</a></li>
          </ul>
        </div>
        <div>
          <h3 class="mb-4 font-bold text-gold-400">Help</h3>
          <ul class="space-y-2 text-sm text-white/80">
            <li><a routerLink="/faq" class="hover:text-gold-400">FAQ</a></li>
            <li><a routerLink="/contact" class="hover:text-gold-400">Contact Us</a></li>
            <li><a routerLink="/terms" class="hover:text-gold-400">Terms</a></li>
            <li><a routerLink="/privacy" class="hover:text-gold-400">Privacy</a></li>
          </ul>
        </div>
        <div>
          <h3 class="mb-4 font-bold text-gold-400">Download App</h3>
          <p class="text-sm text-white/70">Coming soon on Android & iOS</p>
        </div>
      </div>
      <div class="border-t border-white/10 py-5 text-center text-sm text-white/50">
        &copy; {{ year }} Living Lanka. All rights reserved.
      </div>
    </footer>
  `,
})
export class SiteFooterComponent {
  readonly year = new Date().getFullYear();
}
