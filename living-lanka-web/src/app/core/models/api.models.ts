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

export interface CategoryTreeNodeResponse {
  id: string;
  name: string;
  slug: string;
  iconUrl?: string;
  listingCount: number;
  searchTerm?: string;
  subCategories: CategoryTreeNodeResponse[];
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

export interface ListingImageResponse {
  id: string;
  url: string;
  thumbnailUrl?: string;
  altText?: string;
  sortOrder: number;
  isPrimary: boolean;
}

export interface ListingDetailResponse extends ListingResponse {
  locationName?: string;
  latitude?: number;
  longitude?: number;
  images: ListingImageResponse[];
}

export interface ListingSearchParams {
  searchTerm?: string;
  categoryId?: string;
  locationId?: string;
  city?: string;
  province?: string;
  condition?: string;
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
  listingPurpose: string;
  mobilePhone: string;
  whatsAppPhone: string;
  address?: string;
  adDurationDays: number;
  locationId?: string;
  city?: string;
  district?: string;
  province?: string;
  country: string;
  contactPhone?: string;
  contactEmail?: string;
  showPhone: boolean;
  showEmail: boolean;
  latitude?: number;
  longitude?: number;
  expiresAt?: string;
  images?: CreateListingImageRequest[];
}

export interface PaymentCalculationRequest {
  categoryId: string;
  listingPurpose: string;
  durationDays: number;
}

export interface PaymentCalculationResponse {
  categoryId: string;
  categoryName: string;
  listingPurpose: string;
  durationDays: number;
  perDayPrice: number;
  totalAmount: number;
  currency: string;
}

export interface CompletePaymentRequest {
  listingId: string;
  cardNumber: string;
  cardHolderName: string;
  expiryMonth: string;
  expiryYear: string;
  cvv: string;
}

export interface PaymentResponse {
  paymentId: string;
  listingId: string;
  amount: number;
  currency: string;
  status: string;
  transactionRef?: string;
  listingStatus: string;
  paidAt?: string;
}

export interface CategoryPricingResponse {
  id: string;
  name: string;
  slug: string;
  perDayPriceSale: number;
  perDayPriceBuy: number;
  perDayPriceRent: number;
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
  avatarUrl?: string;
  language?: string;
  currency?: string;
  timezone?: string;
}

export interface ApiError {
  error: string;
}
