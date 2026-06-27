export interface IconShape {
  d: string;
  fill?: string;
  stroke?: string;
  strokeWidth?: number;
  fillRule?: 'evenodd' | 'nonzero';
}

export interface CategoryIconDef {
  viewBox?: string;
  shapes: IconShape[];
}

const gray = '#6b7280';
const blue = '#2563eb';
const green = '#16a34a';
const red = '#dc2626';
const amber = '#d97706';
const teal = '#0d9488';
const brown = '#92400e';
const purple = '#7c3aed';

export const CATEGORY_ICONS: Record<string, CategoryIconDef> = {
  // Parent categories
  vehicles: {
    shapes: [
      { d: 'M4 14h16l-1.5-5H5.5L4 14z', fill: green },
      { d: 'M6 14a2 2 0 104 0 2 2 0 00-4 0zm8 0a2 2 0 104 0 2 2 0 00-4 0z', fill: '#15803d' },
    ],
  },
  property: {
    shapes: [
      { d: 'M12 3L4 10v9h16v-9L12 3z', fill: red },
      { d: 'M10 19v-6h4v6', fill: '#fff' },
    ],
  },
  mobiles: {
    shapes: [
      { d: 'M9 2h6a2 2 0 012 2v16a2 2 0 01-2 2H9a2 2 0 01-2-2V4a2 2 0 012-2z', fill: blue },
      { d: 'M11 18h2v1h-2z', fill: '#fff' },
    ],
  },
  electronics: { shapes: [{ d: 'M3 5h18v11H3V5zm2 13h14v2H5v-2z', fill: gray }] },
  services: {
    shapes: [
      { d: 'M14.7 6.3a4 4 0 00-5.4 5.4L4 17v3h3l5.3-5.3a4 4 0 005.4-5.4z', fill: gray, stroke: amber, strokeWidth: 1.5 },
    ],
  },
  'home-garden': { shapes: [{ d: 'M4 10h16v10H4V10zm2-4h12v4H6V6z', fill: teal }] },
  'business-industry': { shapes: [{ d: 'M4 21V9l8-4 8 4v12H4zm4-8h2v8H8v-8zm4 0h2v8h-2v-8zm4 0h2v8h-2v-8z', fill: blue }] },
  jobs: { shapes: [{ d: 'M8 7V5a2 2 0 012-2h4a2 2 0 012 2v2h4v12H4V7h4z', fill: blue }] },
  animals: { shapes: [{ d: 'M12 11c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM6 13c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM18 13c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM8 17c1.5 2 4.5 2 6 0', fill: amber, stroke: amber, strokeWidth: 1.5 }] },
  'hobby-sport-kids': { shapes: [{ d: 'M12 22a10 10 0 100-20 10 10 0 000 20zM12 6v12M6 12h12', fill: gray, stroke: '#111', strokeWidth: 1.5 }] },
  'fashion-beauty': { shapes: [{ d: 'M12 8a4 4 0 100-8 4 4 0 000 8zm-7 8a7 7 0 0114 0v2H5v-2z', fill: gray }] },
  essentials: { shapes: [{ d: 'M6 6h12l1 14H5L6 6zm3-2h6l1 2H8l1-2z', fill: brown }] },
  education: { shapes: [{ d: 'M12 3L2 8l10 5 10-5-10-5zM4 10v6l8 4 8-4v-6', fill: blue }] },
  agriculture: { shapes: [{ d: 'M12 22V8m0 0C12 8 8 4 8 2m4 6c0 0 4-4 4-6m-6 10c-2 2-4 2-6 0m12 0c2 2 4 2 6 0', fill: amber, stroke: amber, strokeWidth: 1.5 }] },
  'work-overseas': { shapes: [{ d: 'M8 7V5a2 2 0 012-2h4a2 2 0 012 2v2h2v12H6V7h2zm8 4a6 6 0 11-12 0 6 6 0 0112 0z', fill: blue }] },
  other: { shapes: [{ d: 'M4 7h16v12H4V7zm2 2v8h12V9H6z', fill: brown }] },

  // Subcategory icons
  all: { shapes: [] },
  cars: { shapes: [{ d: 'M4 14h16l-1.5-5H5.5L4 14zm2 0a2 2 0 104 0 2 2 0 00-4 0zm8 0a2 2 0 104 0 2 2 0 00-4 0z', fill: gray }] },
  motorbikes: { shapes: [{ d: 'M5 16h2l2-6h6l2 4h2M9 16a2 2 0 104 0M15 16a2 2 0 104 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  'three-wheelers': { shapes: [{ d: 'M6 15h12l-1-4H7L6 15zm1 0a2 2 0 103 0M15 15a2 2 0 103 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  bicycles: { shapes: [{ d: 'M6 17a3 3 0 106 0 3 3 0 00-6 0zm10 0a3 3 0 106 0 3 3 0 00-6 0M9 14l3-5 3 5', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  vans: { shapes: [{ d: 'M3 14h18v-3l-2-4H5L3 11v3zm3 0a2 2 0 104 0M15 14a2 2 0 104 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  buses: { shapes: [{ d: 'M4 8h16v10H4V8zm2 12a2 2 0 104 0M16 20a2 2 0 104 0M6 11h12', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  trucks: { shapes: [{ d: 'M2 14h13V8H6L2 11v3zm13 0h7l-3-4h-4v4zm-9 0a2 2 0 104 0M17 14a2 2 0 104 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  'heavy-duty': { shapes: [{ d: 'M4 16h16l-2-6H6L4 16zM7 16a2 2 0 104 0M15 16a2 2 0 104 0', fill: gray }] },
  tractors: { shapes: [{ d: 'M5 15h14M8 15l2-5h4l2 5M7 15a2 2 0 104 0M15 15a2 2 0 104 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  'auto-services': { shapes: [{ d: 'M4 14h16l-1.5-5H5.5L4 14zM14.7 6.3a4 4 0 00-5.4 5.4', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  rentals: { shapes: [{ d: 'M4 14h12v-4l3-2v6M8 14a2 2 0 104 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  'auto-parts': { shapes: [{ d: 'M12 12a4 4 0 100-8 4 4 0 000 8zm-8 8a8 8 0 0116 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  'auto-repair': { shapes: [{ d: 'M14.7 6.3a4 4 0 00-5.4 5.4L4 17v3h3l5.3-5.3', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  boats: { shapes: [{ d: 'M3 16h18l-2-4H5L3 16zM12 6v6', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  land: { shapes: [{ d: 'M4 18l4-8 4 5 4-9 4 12H4z', fill: gray }] },
  house: { shapes: [{ d: 'M12 4L4 11v9h16v-9L12 4z', fill: gray }] },
  apartment: { shapes: [{ d: 'M6 4h12v18H6V4zm3 4h2M9 12h2M9 16h2M15 8h2M15 12h2M15 16h2', fill: gray }] },
  commercial: { shapes: [{ d: 'M5 21V5h14v16H5zm3-12h2v2H8v-2zm4 0h2v2h-2v-2zm4 0h2v2h-2v-2z', fill: gray }] },
  room: { shapes: [{ d: 'M7 4h10v16H7V4zm2 4h6v8H9V8z', fill: gray }] },
  holiday: { shapes: [{ d: 'M4 18l3-6h10l3 6H4zm5-10h6l-2 4H11l-2-4z', fill: gray }] },
  'mobile-phones': { shapes: [{ d: 'M9 3h6v18H9V3zm2 14h2v2h-2v-2z', fill: gray }] },
  'mobile-accessories': { shapes: [{ d: 'M12 2a4 4 0 014 4v2H8V6a4 4 0 014-4zm-4 8h8v12H8V10z', fill: gray }] },
  'mobile-spare-parts': { shapes: [{ d: 'M7 7h10v10H7V7zm2 2v6h6V9H9z', fill: gray }] },
  'smart-watches': { shapes: [{ d: 'M9 4h6l1 4v8l-1 4H9l-1-4V8l1-4zm3 8a2 2 0 100-4 2 2 0 000 4z', fill: gray }] },
  computers: { shapes: [{ d: 'M4 6h16v10H4V6zm0 12h16v2H4v-2z', fill: gray }] },
  headphones: { shapes: [{ d: 'M4 12a8 8 0 0116 0v5a2 2 0 01-2 2h-2v-7h4M4 17a2 2 0 01-2-2v-3h4v7H4z', fill: gray }] },
  tv: { shapes: [{ d: 'M3 6h18v11H3V6zm6 13h6v2H9v-2z', fill: gray }] },
  'tv-4k': { shapes: [{ d: 'M5 5h14v12H5V5zm2 2h10v8H7V7z', fill: gray }] },
  camera: { shapes: [{ d: 'M4 8h4l2-2h4l2 2h4v12H4V8zm8 3a3 3 0 100 6 3 3 0 000-6z', fill: gray }] },
  microphone: { shapes: [{ d: 'M12 14a3 3 0 003-3V6a3 3 0 10-6 0v5a3 3 0 003 3zm0 2v4m-4 0h8', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  appliance: { shapes: [{ d: 'M6 4h12a2 2 0 012 2v12a2 2 0 01-2 2H6a2 2 0 01-2-2V6a2 2 0 012-2zm6 8a2 2 0 100-4 2 2 0 000 4z', fill: gray }] },
  fan: { shapes: [{ d: 'M12 12a2 2 0 100-4 2 2 0 000 4zm0 0v8m-4-4h8', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  gamepad: { shapes: [{ d: 'M6 10h12a4 4 0 010 8h-1l-2-3H9L7 18H6a4 4 0 010-8zm4 3v2m-1-1h2', fill: gray }] },
  speaker: { shapes: [{ d: 'M8 8h8v12H8V8zm2 2v8h4v-8H10z', fill: gray }] },
  furniture: { shapes: [{ d: 'M4 14h16v4H4v-4zm2-8h12v8H6V6z', fill: gray }] },
  bathroom: { shapes: [{ d: 'M8 6h8v4H8V6zm-2 10h12v4H6v-4zM12 10v4', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  garden: { shapes: [{ d: 'M12 22V10m0 0C12 10 8 6 8 4m4 6c0 0 4-4 4-6', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  sofa: { shapes: [{ d: 'M4 12h16v6H4v-6zm2-4h12v4H6V8z', fill: gray }] },
  kitchen: { shapes: [{ d: 'M6 4h12v8H6V4zm0 10h12v6H6v-6z', fill: gray }] },
  chair: { shapes: [{ d: 'M8 10h8v8H8v-8zm2-6h4v6h-4V4z', fill: gray }] },
  pets: { shapes: [{ d: 'M8 12c0-2 1-4 4-4s4 2 4 4-2 4-4 4-4-2-4-4zM6 8a2 2 0 110-4 2 2 0 010 4zm12 0a2 2 0 110-4 2 2 0 010 4z', fill: gray }] },
  'pet-food': { shapes: [{ d: 'M8 8h8v10H8V8zm2 2v6h4v-6h-4z', fill: gray }] },
  vet: { shapes: [{ d: 'M12 11c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM8 17c1.5 2 4.5 2 6 0', fill: gray }] },
  farm: { shapes: [{ d: 'M4 18V8l8-4 8 4v10H4zm4-6h2v6H8v-6zm4 0h2v6h-2v-6zm4 0h2v6h-2v-6z', fill: gray }] },
  rabbit: { shapes: [{ d: 'M10 8c0-2 2-4 4-4v4l3 8H7l3-8z', fill: gray }] },
  paw: { shapes: [{ d: 'M12 11c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM6 13c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM18 13c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2z', fill: gray }] },
  'hard-hat': { shapes: [{ d: 'M6 14h12l-1-5H7l-1 5zm3-8h6v3H9V6z', fill: gray }] },
  spray: { shapes: [{ d: 'M10 4h4v4h-4V4zm-2 6h8v10H8V10z', fill: gray }] },
  stage: { shapes: [{ d: 'M4 18h16M6 18V8l6-4 6 4v10', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  health: { shapes: [{ d: 'M12 8v8M8 12h8', fill: 'none', stroke: gray, strokeWidth: 2 }] },
  airplane: { shapes: [{ d: 'M2 12h20l-8-4v-3l-4 2v5l-8-4z', fill: gray }] },
  gear: { shapes: [{ d: 'M12 8a4 4 0 100 8 4 4 0 000-8zm7 4l2 1-1 2-2-1-1 2-2-1-1 2-2-1-2 1-1-2-2 1-1-2 2-1 1-2 2 1 1-2 2 1 2-1 1 2 2 1 1 2 2-1', fill: gray }] },
  tools: { shapes: [{ d: 'M14.7 6.3a4 4 0 00-5.4 5.4L4 17v3h3l5.3-5.3', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  solar: { shapes: [{ d: 'M4 14h16l-2-6H6L4 14zM8 8V4m4 4V2m4 4V4', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  blocks: { shapes: [{ d: 'M4 10h6v6H4v-6zm10 0h6v6h-6v-6zM9 16h6v6H9v-6z', fill: gray }] },
  certificate: { shapes: [{ d: 'M6 4h12v14H6V4zm2 2v10h8V6H8zm4 12l2 2 2-2', fill: gray }] },
  stethoscope: { shapes: [{ d: 'M8 4h8v4H8V4zm0 6a4 4 0 008 0v2h2a3 3 0 010 6H8', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  wheelbarrow: { shapes: [{ d: 'M4 14h10l2-4H8L4 14zm10 0a2 2 0 104 0M6 18h12', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  handshake: { shapes: [{ d: 'M8 12l2 2 4-4 6 6M4 16l4-4', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  briefcase: { shapes: [{ d: 'M8 7V5a2 2 0 012-2h4a2 2 0 012 2v2h3v12H5V7h3z', fill: gray }] },
  monitor: { shapes: [{ d: 'M4 6h16v10H4V6zm0 12h16v2H4v-2z', fill: gray }] },
  guitar: { shapes: [{ d: 'M12 4v16M9 8h6M8 14h8', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  'table-tennis': { shapes: [{ d: 'M6 14h12M12 6v8m-4 0a4 4 0 008 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  supplement: { shapes: [{ d: 'M10 4h4v16h-4V4zm-4 6h12v4H6v-4z', fill: gray }] },
  tickets: { shapes: [{ d: 'M4 8h16v8H4V8zm4 0v8M16 8v8', fill: gray }] },
  palette: { shapes: [{ d: 'M12 3a9 9 0 100 18h2a2 2 0 002-2v-1a2 2 0 012-2h1a4 4 0 000-8h-1a9 9 0 00-6 5z', fill: gray }] },
  cd: { shapes: [{ d: 'M12 4a8 8 0 100 16 8 8 0 000-16zm0 4a4 4 0 100 8 4 4 0 000-8z', fill: gray }] },
  teddy: { shapes: [{ d: 'M8 10a4 4 0 018 0v6H8v-6zm-2-2a2 2 0 110-4 2 2 0 010 4zm12 0a2 2 0 110-4 2 2 0 010 4z', fill: gray }] },
  puzzle: { shapes: [{ d: 'M8 8h4v4H8V8zm4 0h4v4h-4V8zm-4 4h4v4H8v-4z', fill: gray }] },
  handbag: { shapes: [{ d: 'M7 8h10v12H7V8zm2-4h6v4H9V4z', fill: gray }] },
  hanger: { shapes: [{ d: 'M6 10l6-4 6 4v2H6v-2zM12 6v12', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  shoe: { shapes: [{ d: 'M4 14h16l-2-4H6L4 14z', fill: gray }] },
  diamond: { shapes: [{ d: 'M12 4l8 8-8 8-8-8 8-8z', fill: gray }] },
  glasses: { shapes: [{ d: 'M4 12h4a2 2 0 104 0H8m8 0h4a2 2 0 104 0h-4', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  watch: { shapes: [{ d: 'M9 4h6l1 4v8l-1 4H9l-1-4V8l1-4z', fill: gray }] },
  cosmetic: { shapes: [{ d: 'M10 6h4v14h-4V6zm-2 4h8v2H8v-2z', fill: gray }] },
  person: { shapes: [{ d: 'M12 12a4 4 0 100-8 4 4 0 000 8zm-6 8a6 6 0 0112 0', fill: gray }] },
  basket: { shapes: [{ d: 'M6 8h12l-2 12H8L6 8zm3-4h6l1 4H8l1-4z', fill: gray }] },
  fruits: { shapes: [{ d: 'M8 10a4 4 0 018 0v8H8v-8zM12 6V4', fill: gray }] },
  fish: { shapes: [{ d: 'M4 12c4-4 8-4 12 0s-8 4-12 0zm14 0l4 2-4 2', fill: gray }] },
  baby: { shapes: [{ d: 'M12 8a3 3 0 100-6 3 3 0 000 6zm-5 10a5 5 0 0110 0', fill: gray }] },
  'health-cross': { shapes: [{ d: 'M12 8v8M8 12h8', fill: 'none', stroke: gray, strokeWidth: 2 }] },
  'house-small': { shapes: [{ d: 'M12 5L5 11v8h14v-8L12 5z', fill: gray }] },
  gas: { shapes: [{ d: 'M10 4h4v16h-4V4zm-4 6h12v4H6v-4z', fill: gray }] },
  cart: { shapes: [{ d: 'M6 8h14l-2 10H8L6 8zm3 12a2 2 0 104 0M15 20a2 2 0 104 0', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  podium: { shapes: [{ d: 'M8 18h8v4H8v-4zm2-6h4v6h-4v-6zM12 4v4', fill: gray }] },
  'book-closed': { shapes: [{ d: 'M6 4h12v16H6V4zm2 2v12h8V6H8z', fill: gray }] },
  tuition: { shapes: [{ d: 'M12 4v16M8 8h8M8 12h8', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  vocational: { shapes: [{ d: 'M12 4l8 4v12H4V8l8-4z', fill: gray }] },
  'book-open': { shapes: [{ d: 'M4 6h7v14H4V6zm9 0h7v14h-7V6z', fill: gray }] },
  sprout: { shapes: [{ d: 'M12 22V10m0 0C12 10 8 6 8 4m4 6c0 0 4-4 4-6', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  rake: { shapes: [{ d: 'M8 4v16M12 8v12M16 4v16', fill: 'none', stroke: gray, strokeWidth: 1.8 }] },
  fence: { shapes: [{ d: 'M4 10h16v2H4v-2zm2 6h2v4H6v-4zm4 0h2v4h-2v-4zm4 0h2v4h-2v-4z', fill: gray }] },
  'suitcase-globe': { shapes: [{ d: 'M8 7V5a2 2 0 012-2h4a2 2 0 012 2v2h2v12H6V7h2zm6 4a4 4 0 11-8 0 4 4 0 018 0z', fill: gray }] },
  'study-abroad': { shapes: [{ d: 'M12 3L2 8l10 5 10-5-10-5zM4 10v6l8 4 8-4v-6', fill: gray }] },
  car: { shapes: [{ d: 'M4 14h16l-1.5-5H5.5L4 14z', fill: gray }] },
};

export function getCategoryIcon(key?: string | null): CategoryIconDef {
  if (!key) {
    return { shapes: [{ d: 'M6 4h12v16H6V4z', fill: gray }] };
  }

  const normalized = key.replace(/^\/icons\//, '').replace(/\.svg$/, '').toLowerCase();
  return CATEGORY_ICONS[normalized] ?? CATEGORY_ICONS[key] ?? {
    shapes: [{ d: 'M6 4h12v16H6V4z', fill: gray }],
  };
}

export function iconKeyFromCategory(slug: string, iconUrl?: string | null): string {
  if (iconUrl && !iconUrl.startsWith('/icons/')) {
    return iconUrl;
  }
  return slug;
}
