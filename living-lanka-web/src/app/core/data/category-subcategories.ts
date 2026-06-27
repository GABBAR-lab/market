export interface SubCategoryLink {
  label: string;
  slug: string;
  searchTerm?: string;
}

export interface CategoryQuickLinks {
  slug: string;
  title: string;
  subcategories: SubCategoryLink[];
}

export const CATEGORY_QUICK_LINKS: CategoryQuickLinks[] = [
  {
    slug: 'electronics',
    title: 'Electronics',
    subcategories: [
      { label: 'Mobile Phones', slug: 'mobiles', searchTerm: 'phone' },
      { label: 'Cameras', slug: 'electronics', searchTerm: 'camera' },
      { label: 'Computers & Tablets', slug: 'electronics', searchTerm: 'computer' },
      { label: 'TVs', slug: 'electronics', searchTerm: 'tv' },
      { label: 'Air Conditions', slug: 'electronics', searchTerm: 'ac' },
    ],
  },
  {
    slug: 'property',
    title: 'Property',
    subcategories: [
      { label: 'Land', slug: 'property', searchTerm: 'land' },
      { label: 'Houses For Sale', slug: 'property', searchTerm: 'house sale' },
      { label: 'House Rentals', slug: 'property', searchTerm: 'rent' },
      { label: 'Apartments For Sale', slug: 'property', searchTerm: 'apartment sale' },
      { label: 'Apartment Rentals', slug: 'property', searchTerm: 'apartment rent' },
    ],
  },
  {
    slug: 'jobs',
    title: 'Jobs',
    subcategories: [
      { label: 'Data Entry', slug: 'jobs', searchTerm: 'data entry' },
      { label: 'Driver', slug: 'jobs', searchTerm: 'driver' },
      { label: 'Clerk', slug: 'jobs', searchTerm: 'clerk' },
      { label: 'Sales Executive', slug: 'jobs', searchTerm: 'sales' },
      { label: 'IT & Software', slug: 'jobs', searchTerm: 'software' },
    ],
  },
  {
    slug: 'vehicles',
    title: 'Vehicles',
    subcategories: [
      { label: 'Cars', slug: 'vehicles', searchTerm: 'car' },
      { label: 'Motorbikes', slug: 'vehicles', searchTerm: 'bike' },
      { label: 'Three Wheelers', slug: 'vehicles', searchTerm: 'tuk' },
      { label: 'Auto Parts', slug: 'vehicles', searchTerm: 'parts' },
      { label: 'Auto Services', slug: 'services', searchTerm: 'auto' },
    ],
  },
];
