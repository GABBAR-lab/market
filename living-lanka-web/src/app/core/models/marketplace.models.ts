export interface Category {
  id: string;
  title: string;
  subtitle?: string;
  icon: string;
  iconUrl?: string;
  slug: string;
  adCount?: number;
}

export interface CategoryTreeNode {
  id: string;
  name: string;
  slug: string;
  iconUrl?: string;
  listingCount: number;
  searchTerm?: string;
  subCategories: CategoryTreeNode[];
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
  imageUrls?: string[];
  category: string;
  categoryId?: string;
  sellerId?: string;
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
  latitude?: number;
  longitude?: number;
  publishedAt?: string;
  bedrooms?: number;
  bathrooms?: number;
  areaSqFt?: number;
}

export interface SearchFilters {
  query?: string;
  location?: string;
  province?: string;
  propertyType?: string;
  category?: string;
  categoryId?: string;
  posterType?: string;
  listingType?: 'sale' | 'rent' | 'buy';
  minPrice?: number;
  maxPrice?: number;
  city?: string;
  condition?: string;
  sortBy?: 'newest' | 'oldest' | 'price_asc' | 'price_desc';
}

export interface PublicSellerProfile {
  userId: string;
  displayName: string;
  avatarUrl?: string;
  bio?: string;
  memberSince: string;
}

export interface AuthUser {
  userId: string;
  email: string;
  firstName?: string;
  lastName?: string;
}
