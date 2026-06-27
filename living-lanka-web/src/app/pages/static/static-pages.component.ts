import { Component } from '@angular/core';
import { StaticPageComponent } from '../../shared/components/static-page/static-page.component';

@Component({
  selector: 'app-about',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="About Living Lanka">
      <p>
        Living Lanka is Sri Lanka's growing online marketplace — buy and sell anything from vehicles and
        property to electronics, jobs, and services across the island.
      </p>
      <p class="mt-4">
        Inspired by leading platforms like <a href="https://ikman.lk" target="_blank" rel="noopener" class="text-teal-700 underline">ikman.lk</a>,
        we connect buyers and sellers with a simple, free ad posting experience.
      </p>
    </app-static-page>
  `,
})
export class AboutComponent {}

@Component({
  selector: 'app-contact',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="Contact Us">
      <p><strong>Email:</strong> support@livinglanka.lk</p>
      <p class="mt-2"><strong>Phone:</strong> +94 11 234 5678</p>
      <p class="mt-2"><strong>Address:</strong> Colombo, Sri Lanka</p>
      <p class="mt-4 text-gray-600">We typically respond within 24–48 hours on business days.</p>
    </app-static-page>
  `,
})
export class ContactComponent {}

@Component({
  selector: 'app-faq',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="FAQ">
      <h3 class="font-bold">How do I post an ad?</h3>
      <p>Register, login, and click POST YOUR AD. Fill the form and publish — it's free!</p>
      <h3 class="mt-4 font-bold">How long does my ad stay active?</h3>
      <p>You choose duration: 7 to 180 days when posting.</p>
      <h3 class="mt-4 font-bold">Is it safe to buy on Living Lanka?</h3>
      <p>Meet in public places, verify items, and never pay in advance without seeing the product.</p>
    </app-static-page>
  `,
})
export class FaqComponent {}

@Component({
  selector: 'app-terms',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="Terms & Conditions">
      <p>By using Living Lanka you agree to post accurate listings, respect other users, and comply with Sri Lankan law.</p>
      <p class="mt-4">We reserve the right to remove ads that violate our policies.</p>
    </app-static-page>
  `,
})
export class TermsComponent {}

@Component({
  selector: 'app-privacy',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="Privacy Policy">
      <p>We collect email, phone, and listing data to operate the marketplace. We do not sell your personal data.</p>
      <p class="mt-4">Contact us to request data deletion.</p>
    </app-static-page>
  `,
})
export class PrivacyComponent {}

@Component({
  selector: 'app-sell-fast',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="Sell Fast">
      <ul class="list-disc space-y-2 pl-5">
        <li>Use clear photos and honest titles</li>
        <li>Set a fair price — research similar ads</li>
        <li>Fill location and WhatsApp number</li>
        <li>Choose the right category</li>
        <li>Reply quickly to buyers</li>
      </ul>
    </app-static-page>
  `,
})
export class SellFastComponent {}

@Component({
  selector: 'app-membership',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="Membership">
      <p>Membership perks coming soon: featured ads, verified badge, and priority support.</p>
      <p class="mt-4">For now, all basic ads are <strong>free</strong>.</p>
    </app-static-page>
  `,
})
export class MembershipComponent {}

@Component({
  selector: 'app-chat',
  imports: [StaticPageComponent],
  template: `
    <app-static-page title="Chat">
      <p>In-app messaging is coming soon. For now, contact sellers via WhatsApp or phone on each listing.</p>
    </app-static-page>
  `,
})
export class ChatComponent {}
