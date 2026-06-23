export interface Category {
  id: string;
  title: string;
  subtitle?: string;
  icon: string;
  slug: string;
  adCount?: number;
}

export interface NavItem {
  label: string;
  slug: string;
  children?: { label: string; slug: string }[];
}

export interface PropertyListing {
  id: string;
  title: string;
  slug: string;
  price: number;
  currency: string;
  location: string;
  imageUrl: string;
  category: string;
  categoryId?: string;
  description?: string;
  postedBy?: string;
  isVerified?: boolean;
  isFeatured: boolean;
  badge?: 'new' | 'hot';
  postedAt?: string;
  status?: string;
  viewCount?: number;
  contactPhone?: string;
  contactEmail?: string;
  showPhone?: boolean;
  showEmail?: boolean;
  priceType?: string;
  condition?: string;
  bedrooms?: number;
  bathrooms?: number;
  areaSqFt?: number;
}

export interface SearchFilters {
  query?: string;
  location?: string;
  propertyType?: string;
  category?: string;
  categoryId?: string;
  posterType?: string;
  listingType?: 'sale' | 'rent';
  minPrice?: number;
  maxPrice?: number;
  city?: string;
}

export interface AuthUser {
  userId: string;
  email: string;
  firstName?: string;
  lastName?: string;
}
