import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-site-footer',
  imports: [RouterLink],
  template: `
    <footer class="mt-auto border-t border-gray-200 bg-gray-50">
      <div class="section-container grid grid-cols-1 gap-10 py-12 sm:grid-cols-2 lg:grid-cols-4">
        <div>
          <h3 class="mb-4 font-bold text-gray-900">More from Living Lanka</h3>
          <ul class="space-y-2 text-sm text-gray-600">
            <li><a routerLink="/sell-fast" class="hover:text-teal-700">Sell Fast</a></li>
            <li><a routerLink="/membership" class="hover:text-teal-700">Membership</a></li>
            <li><a routerLink="/post-ad" class="hover:text-teal-700">Post Free Ad</a></li>
          </ul>
        </div>
        <div>
          <h3 class="mb-4 font-bold text-gray-900">Help & Support</h3>
          <ul class="space-y-2 text-sm text-gray-600">
            <li><a routerLink="/faq" class="hover:text-teal-700">FAQ</a></li>
            <li><a routerLink="/contact" class="hover:text-teal-700">Contact Us</a></li>
            <li><a routerLink="/terms" class="hover:text-teal-700">Terms & Conditions</a></li>
          </ul>
        </div>
        <div>
          <h3 class="mb-4 font-bold text-gray-900">About</h3>
          <ul class="space-y-2 text-sm text-gray-600">
            <li><a routerLink="/about" class="hover:text-teal-700">About Us</a></li>
            <li><a routerLink="/privacy" class="hover:text-teal-700">Privacy Policy</a></li>
            <li><a routerLink="/all-ads" class="hover:text-teal-700">All Ads</a></li>
          </ul>
        </div>
        <div>
          <h3 class="mb-4 font-bold text-gray-900">Download our app</h3>
          <p class="text-sm text-gray-600">Coming soon on Android & iOS</p>
          <div class="mt-4 flex gap-2">
            <span class="rounded-lg bg-gray-200 px-4 py-2 text-xs font-medium text-gray-600">Google Play</span>
            <span class="rounded-lg bg-gray-200 px-4 py-2 text-xs font-medium text-gray-600">App Store</span>
          </div>
        </div>
      </div>
      <div class="border-t border-gray-200 bg-white py-6 text-center text-sm text-gray-500">
        &copy; {{ year }} Living Lanka. All rights reserved. Sri Lanka's Marketplace.
      </div>
    </footer>
  `,
})
export class SiteFooterComponent {
  readonly year = new Date().getFullYear();
}
