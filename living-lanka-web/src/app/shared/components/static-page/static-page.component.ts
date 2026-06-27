import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-static-page',
  imports: [RouterLink],
  template: `
    <div class="section-container py-12">
      <h1 class="text-3xl font-bold text-gray-900">{{ title }}</h1>
      <div class="prose prose-gray mt-6 max-w-3xl">
        <ng-content />
      </div>
      <a routerLink="/" class="btn mt-8 bg-maroon-800 text-white">Back to Home</a>
    </div>
  `,
})
export class StaticPageComponent {
  @Input({ required: true }) title!: string;
}
