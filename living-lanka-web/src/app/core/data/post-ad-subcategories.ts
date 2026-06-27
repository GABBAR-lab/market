import { ListingPurpose } from './sri-lanka-locations';

export interface PostAdSubCategory {
  label: string;
  slug: string;
  categorySlug: string;
}

export const POST_AD_SUBCATEGORIES: Record<ListingPurpose, PostAdSubCategory[]> = {
  Sale: [
    { label: 'Vehicle', slug: 'vehicle', categorySlug: 'vehicles' },
    { label: 'Land', slug: 'land', categorySlug: 'property' },
    { label: 'House', slug: 'house', categorySlug: 'property' },
    { label: 'Mobile', slug: 'mobile', categorySlug: 'mobiles' },
    { label: 'Electronics', slug: 'electronics', categorySlug: 'electronics' },
    { label: 'Shop', slug: 'shop', categorySlug: 'business-industry' },
    { label: 'Other', slug: 'other', categorySlug: 'other' },
  ],
  Rent: [
    { label: 'House', slug: 'house-rent', categorySlug: 'property' },
    { label: 'Vehicle', slug: 'vehicle-rent', categorySlug: 'vehicles' },
    { label: 'Shop', slug: 'shop-rent', categorySlug: 'business-industry' },
    { label: 'Apartment', slug: 'apartment-rent', categorySlug: 'property' },
    { label: 'Land', slug: 'land-rent', categorySlug: 'property' },
  ],
  Buy: [
    { label: 'Vehicle', slug: 'vehicle-buy', categorySlug: 'vehicles' },
    { label: 'Land', slug: 'land-buy', categorySlug: 'property' },
    { label: 'Mobile', slug: 'mobile-buy', categorySlug: 'mobiles' },
    { label: 'House', slug: 'house-buy', categorySlug: 'property' },
    { label: 'Electronics', slug: 'electronics-buy', categorySlug: 'electronics' },
  ],
};

export const AD_DURATION_PRESETS = [7, 14, 30, 60, 90] as const;
