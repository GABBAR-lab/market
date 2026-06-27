import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ListingApiService } from '../../core/services/listing-api.service';
import { CategoryPricingResponse } from '../../core/models/api.models';

@Component({
  selector: 'app-admin-pricing',
  imports: [RouterLink, FormsModule],
  template: `
    <div class="section-container py-10">
      <h1 class="mb-6 text-2xl font-bold">Admin — Category Pricing (Per Day)</h1>
      @if (error()) {
        <div class="alert alert-error mb-4">{{ error() }}</div>
      }
      @if (saved()) {
        <div class="alert alert-success mb-4">Pricing updated.</div>
      }
      <div class="overflow-x-auto rounded-lg border">
        <table class="table">
          <thead>
            <tr>
              <th>Category</th>
              <th>Sale (Rs/day)</th>
              <th>Buy (Rs/day)</th>
              <th>Rent (Rs/day)</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            @for (row of rows(); track row.id) {
              <tr>
                <td class="font-medium">{{ row.name }}</td>
                <td><input type="number" [(ngModel)]="row.perDayPriceSale" class="input input-bordered input-sm w-24" min="0" /></td>
                <td><input type="number" [(ngModel)]="row.perDayPriceBuy" class="input input-bordered input-sm w-24" min="0" /></td>
                <td><input type="number" [(ngModel)]="row.perDayPriceRent" class="input input-bordered input-sm w-24" min="0" /></td>
                <td>
                  <button type="button" class="btn btn-sm btn-primary" (click)="save(row)">Save</button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
      <a routerLink="/" class="btn btn-ghost mt-6">Back to Home</a>
    </div>
  `,
})
export class AdminPricingComponent implements OnInit {
  private readonly api = inject(ListingApiService);
  readonly rows = signal<CategoryPricingResponse[]>([]);
  readonly error = signal('');
  readonly saved = signal(false);

  ngOnInit(): void {
    this.api.getCategoryPricing().subscribe({
      next: (items) => this.rows.set(items),
      error: () => this.error.set('Failed to load pricing. Admin login required.'),
    });
  }

  save(row: CategoryPricingResponse): void {
    this.saved.set(false);
    this.api.updateCategoryPricing(row.id, {
      perDayPriceSale: row.perDayPriceSale,
      perDayPriceBuy: row.perDayPriceBuy,
      perDayPriceRent: row.perDayPriceRent,
    }).subscribe({
      next: () => this.saved.set(true),
      error: () => this.error.set('Failed to save pricing.'),
    });
  }
}
