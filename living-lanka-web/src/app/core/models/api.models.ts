export interface AuthResponse {
  userId: string;
  email: string;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface CategoryResponse {
  id: string;
  name: string;
  slug: string;
  description?: string;
  iconUrl?: string;
  parentCategoryId?: string;
  sortOrder: number;
  isActive: boolean;
  listingCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface ListingResponse {
  id: string;
  sellerId: string;
  categoryId: string;
  categoryName: string;
  title: string;
  slug: string;
  description: string;
  price?: number;
  currency: string;
  priceType: string;
  condition: string;
  status: string;
  locationId?: string;
  city?: string;
  district?: string;
  province?: string;
  country: string;
  contactPhone?: string;
  contactEmail?: string;
  showPhone: boolean;
  showEmail: boolean;
  viewCount: number;
  isFeatured: boolean;
  featuredUntil?: string;
  publishedAt?: string;
  expiresAt?: string;
  primaryImageUrl?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface ListingSearchParams {
  searchTerm?: string;
  categoryId?: string;
  locationId?: string;
  city?: string;
  minPrice?: number;
  maxPrice?: number;
  status?: string;
  isFeatured?: boolean;
  sellerId?: string;
  sortBy?: string;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateListingRequest {
  categoryId: string;
  title: string;
  slug: string;
  description: string;
  price?: number;
  currency: string;
  priceType: string;
  condition: string;
  locationId?: string;
  city?: string;
  district?: string;
  province?: string;
  country: string;
  contactPhone?: string;
  contactEmail?: string;
  showPhone: boolean;
  showEmail: boolean;
  expiresAt?: string;
  images?: CreateListingImageRequest[];
}

export interface CreateListingImageRequest {
  url: string;
  thumbnailUrl?: string;
  altText?: string;
  sortOrder: number;
  isPrimary: boolean;
}

export interface LocationResponse {
  id: string;
  name: string;
  slug: string;
  type: string;
  parentLocationId?: string;
  sortOrder: number;
  isActive: boolean;
  children?: LocationResponse[];
  createdAt: string;
  updatedAt?: string;
}

export interface ProfileResponse {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  bio?: string;
  avatarUrl?: string;
  dateOfBirth?: string;
  gender?: string;
  phoneNumber?: string;
  website?: string;
  language: string;
  currency: string;
  timezone: string;
  emailNotifications: boolean;
  smsNotifications: boolean;
  status: string;
  createdAt: string;
  updatedAt?: string;
  addresses: AddressResponse[];
}

export interface AddressResponse {
  id: string;
  label: string;
  streetLine1: string;
  streetLine2?: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
  isDefault: boolean;
  createdAt: string;
}

export interface CreateProfileRequest {
  userId: string;
  firstName: string;
  lastName: string;
  bio?: string;
  phoneNumber?: string;
  language?: string;
  currency?: string;
  timezone?: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  bio?: string;
  phoneNumber?: string;
  language?: string;
  currency?: string;
  timezone?: string;
}

export interface ApiError {
  error: string;
}
