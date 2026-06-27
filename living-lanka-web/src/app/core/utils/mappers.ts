import { CategoryResponse, CategoryTreeNodeResponse, ListingDetailResponse, ListingResponse } from '../models/api.models';
import { Category, CategoryTreeNode, PropertyListing } from '../models/marketplace.models';
import { iconKeyFromCategory } from '../data/category-icons';

const SLUG_ICON_MAP: Record<string, string> = {
  vehicles: 'car',
  property: 'home',
  electronics: 'monitor',
  mobiles: 'phone',
  services: 'wrench',
  'home-garden': 'sofa',
  'business-industry': 'building',
  jobs: 'briefcase',
  animals: 'paw',
  'hobby-sport-kids': 'ball',
  'fashion-beauty': 'shirt',
  education: 'graduation',
  essentials: 'bag',
  agriculture: 'leaf',
  'work-overseas': 'globe',
  other: 'bookmark',
};

const DEFAULT_IMAGE =
  'https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=600&q=80';

export function slugToIcon(slug: string): string {
  return SLUG_ICON_MAP[slug] ?? 'bookmark';
}

export function mapCategory(c: CategoryResponse): Category {
  const iconKey = iconKeyFromCategory(c.slug, c.iconUrl);
  return {
    id: c.id,
    title: c.name,
    subtitle: `${c.listingCount.toLocaleString()} ads`,
    icon: SLUG_ICON_MAP[c.slug] ?? iconKey,
    iconUrl: c.iconUrl ?? iconKey,
    slug: c.slug,
    adCount: c.listingCount,
  };
}

export function mapCategoryTreeNode(node: CategoryTreeNodeResponse): CategoryTreeNode {
  return {
    id: node.id,
    name: node.name,
    slug: node.slug,
    iconUrl: node.iconUrl ?? node.slug,
    listingCount: node.listingCount,
    searchTerm: node.searchTerm,
    subCategories: node.subCategories.map(mapCategoryTreeNode),
  };
}

export function mapListing(l: ListingResponse): PropertyListing {
  const location =
    [l.city, l.district, l.province].filter(Boolean).join(', ') || l.country;
  const published = l.publishedAt ? new Date(l.publishedAt) : new Date(l.createdAt);
  const isNew = Date.now() - published.getTime() < 7 * 24 * 60 * 60 * 1000;

  return {
    id: l.id,
    title: l.title,
    slug: l.slug,
    price: l.price ?? 0,
    currency: l.currency,
    location,
    imageUrl: l.primaryImageUrl || DEFAULT_IMAGE,
    category: l.categoryName,
    categoryId: l.categoryId,
    sellerId: l.sellerId,
    description: l.description,
    isFeatured: l.isFeatured,
    badge: l.isFeatured ? 'hot' : isNew ? 'new' : undefined,
    postedAt: formatRelative(published),
    status: l.status,
    viewCount: l.viewCount,
    contactPhone: l.contactPhone,
    contactEmail: l.contactEmail,
    showPhone: l.showPhone,
    showEmail: l.showEmail,
    priceType: l.priceType,
    condition: l.condition,
    publishedAt: l.publishedAt,
  };
}

export function mapListingDetail(l: ListingDetailResponse): PropertyListing {
  const base = mapListing(l);
  const imageUrls = (l.images ?? [])
    .sort((a, b) => a.sortOrder - b.sortOrder)
    .map((img) => img.url);
  return {
    ...base,
    imageUrls: imageUrls.length ? imageUrls : [base.imageUrl],
    imageUrl: imageUrls[0] ?? base.imageUrl,
    latitude: l.latitude,
    longitude: l.longitude,
  };
}

export function slugify(text: string): string {
  return text
    .toLowerCase()
    .trim()
    .replace(/[^\w\s-]/g, '')
    .replace(/[\s_]+/g, '-')
    .replace(/-+/g, '-');
}

function formatRelative(date: Date): string {
  const diffMs = Date.now() - date.getTime();
  const days = Math.floor(diffMs / (1000 * 60 * 60 * 24));
  if (days === 0) return 'Today';
  if (days === 1) return 'Yesterday';
  if (days < 7) return `${days} days ago`;
  if (days < 30) return `${Math.floor(days / 7)} weeks ago`;
  return date.toLocaleDateString('en-LK', { month: 'short', day: 'numeric' });
}
