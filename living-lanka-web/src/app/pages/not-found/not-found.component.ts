import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink],
  template: `
    <div class="flex min-h-[50vh] flex-col items-center justify-center px-4 py-16 text-center">
      <h1 class="text-6xl font-bold text-maroon-800">404</h1>
      <p class="mt-4 text-xl text-gray-600">Page not found</p>
      <a routerLink="/" class="btn btn-success mt-8">Back to Home</a>
    </div>
  `,
})
export class NotFoundComponent {}
