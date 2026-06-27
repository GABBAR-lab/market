import { Component } from '@angular/core';
import { CategoryAccordionComponent } from '../../shared/components/category-accordion/category-accordion.component';

@Component({
  selector: 'app-categories',
  imports: [CategoryAccordionComponent],
  template: `
    <div class="border-b border-gray-200 bg-gray-50 py-8">
      <div class="section-container">
        <h1 class="text-2xl font-bold text-gray-900">Browse by category</h1>
        <p class="mt-1 text-gray-600">All categories — tap to expand subcategories</p>
      </div>
    </div>

    <div class="section-container py-8">
      <app-category-accordion />
    </div>
  `,
})
export class CategoriesComponent {}
