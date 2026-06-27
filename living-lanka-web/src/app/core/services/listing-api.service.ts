import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CategoryResponse,
  CategoryTreeNodeResponse,
  CreateListingRequest,
  ListingResponse,
  ListingSearchParams,
  LocationResponse,
  PagedResult,
  PaymentCalculationRequest,
  PaymentCalculationResponse,
  CompletePaymentRequest,
  PaymentResponse,
  CategoryPricingResponse,
  ListingDetailResponse,
} from '../models/api.models';
import { Category, CategoryTreeNode, PropertyListing, SearchFilters } from '../models/marketplace.models';
import { mapCategory, mapCategoryTreeNode, mapListing, mapListingDetail } from '../utils/mappers';

@Injectable({ providedIn: 'root' })
export class ListingApiService {
  private readonly http = inject(HttpClient);
  private readonly listingsBase = `${environment.apiBaseUrl}/listings`;
  private readonly categoriesBase = `${environment.apiBaseUrl}/categories`;
  private readonly locationsBase = `${environment.apiBaseUrl}/locations`;

  getCategories(): Observable<Category[]> {
    return this.http
      .get<CategoryResponse[]>(this.categoriesBase)
      .pipe(map((items) => items.filter((c) => c.isActive).map(mapCategory)));
  }

  getCategoryTree(): Observable<CategoryTreeNode[]> {
    return this.http
      .get<CategoryTreeNodeResponse[]>(`${this.categoriesBase}/tree`)
      .pipe(map((items) => items.map(mapCategoryTreeNode)));
  }

  getCategoryBySlug(slug: string): Observable<CategoryResponse> {
    return this.http.get<CategoryResponse>(`${this.categoriesBase}/slug/${slug}`);
  }

  getFeaturedListings(limit = 12): Observable<PropertyListing[]> {
    return this.http
      .get<ListingResponse[]>(`${this.listingsBase}/featured`, {
        params: { limit: String(limit) },
      })
      .pipe(map((items) => items.map(mapListing)));
  }

  searchListings(filters: SearchFilters, page = 1, pageSize = 20): Observable<PagedResult<PropertyListing>> {
    const params = this.buildSearchParams(filters, page, pageSize);
    return this.http
      .get<PagedResult<ListingResponse>>(this.listingsBase, { params })
      .pipe(
        map((result) => ({
          ...result,
          items: result.items.map(mapListing),
        }))
      );
  }

  getListingById(id: string): Observable<PropertyListing> {
    return this.http
      .get<ListingDetailResponse>(`${this.listingsBase}/${id}`)
      .pipe(map(mapListingDetail));
  }

  getListingDetail(id: string): Observable<PropertyListing> {
    return this.getListingById(id);
  }

  getListingBySlug(slug: string): Observable<PropertyListing> {
    return this.http
      .get<ListingResponse>(`${this.listingsBase}/slug/${slug}`)
      .pipe(map(mapListing));
  }

  getMyListings(): Observable<PropertyListing[]> {
    return this.http
      .get<ListingResponse[]>(`${this.listingsBase}/me`)
      .pipe(map((items) => items.map(mapListing)));
  }

  createListing(request: CreateListingRequest): Observable<ListingResponse> {
    return this.http.post<ListingResponse>(this.listingsBase, request);
  }

  calculatePayment(request: PaymentCalculationRequest): Observable<PaymentCalculationResponse> {
    return this.http.post<PaymentCalculationResponse>(`${environment.apiBaseUrl}/payments/calculate`, request);
  }

  completePayment(request: CompletePaymentRequest): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(`${environment.apiBaseUrl}/payments/complete`, request);
  }

  getCategoryPricing(): Observable<CategoryPricingResponse[]> {
    return this.http.get<CategoryPricingResponse[]>(`${environment.apiBaseUrl}/admin/categories/pricing`);
  }

  updateCategoryPricing(id: string, body: { perDayPriceSale: number; perDayPriceBuy: number; perDayPriceRent: number }): Observable<CategoryPricingResponse> {
    return this.http.put<CategoryPricingResponse>(`${environment.apiBaseUrl}/admin/categories/${id}/pricing`, body);
  }

  submitForReview(id: string): Observable<ListingResponse> {
    return this.http.post<ListingResponse>(`${this.listingsBase}/${id}/submit`, {});
  }

  incrementViewCount(id: string): Observable<ListingResponse> {
    return this.http.post<ListingResponse>(`${this.listingsBase}/${id}/view`, {});
  }

  deleteListing(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.listingsBase}/${id}`);
  }

  getLocations(): Observable<LocationResponse[]> {
    return this.http.get<LocationResponse[]>(this.locationsBase);
  }

  getLocationsByType(type: string): Observable<LocationResponse[]> {
    return this.http.get<LocationResponse[]>(`${this.locationsBase}/type/${type}`);
  }

  private buildSearchParams(
    filters: SearchFilters,
    page: number,
    pageSize: number
  ): HttpParams {
    const p: ListingSearchParams = {
      page,
      pageSize,
      sortBy: filters.sortBy ?? 'newest',
      status: 'Active',
    };

    if (filters.query) p.searchTerm = filters.query;
    if (filters.categoryId) p.categoryId = filters.categoryId;
    if (filters.city || filters.location || filters.province) {
      p.city = filters.city ?? filters.location ?? filters.province;
    }
    if (filters.minPrice) p.minPrice = filters.minPrice;
    if (filters.maxPrice) p.maxPrice = filters.maxPrice;
    if (filters.listingType === 'rent') p.searchTerm = (p.searchTerm ?? '') + ' rent';
    if (filters.listingType === 'sale') p.searchTerm = (p.searchTerm ?? '') + ' sale';

    let params = new HttpParams();
    Object.entries(p).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return params;
  }
}
