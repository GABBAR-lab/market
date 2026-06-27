export interface SriLankaArea {
  name: string;
}

export interface SriLankaDistrict {
  name: string;
  areas: string[];
}

export interface SriLankaProvince {
  name: string;
  districts: SriLankaDistrict[];
}

export const LISTING_PURPOSES = ['Sale', 'Buy', 'Rent'] as const;
export type ListingPurpose = (typeof LISTING_PURPOSES)[number];

export const AD_DURATION_DAYS = [7, 14, 30, 60, 90, 180] as const;

export const SRI_LANKA_PROVINCES: SriLankaProvince[] = [
  {
    name: 'Western Province',
    districts: [
      {
        name: 'Colombo',
        areas: ['Colombo 01', 'Colombo 02', 'Colombo 03', 'Colombo 04', 'Colombo 05', 'Colombo 06', 'Colombo 07', 'Dehiwala', 'Mount Lavinia', 'Moratuwa', 'Battaramulla', 'Rajagiriya', 'Nugegoda', 'Maharagama'],
      },
      {
        name: 'Gampaha',
        areas: ['Negombo', 'Gampaha', 'Ja-Ela', 'Wattala', 'Kadawatha', 'Ragama', 'Kelaniya', 'Minuwangoda'],
      },
      {
        name: 'Kalutara',
        areas: ['Kalutara', 'Panadura', 'Horana', 'Beruwala', 'Wadduwa', 'Bandaragama'],
      },
    ],
  },
  {
    name: 'Central Province',
    districts: [
      {
        name: 'Kandy',
        areas: ['Kandy City', 'Peradeniya', 'Katugastota', 'Gampola', 'Kundasale', 'Akurana'],
      },
      {
        name: 'Matale',
        areas: ['Matale', 'Dambulla', 'Sigiriya', 'Galewela'],
      },
      {
        name: 'Nuwara Eliya',
        areas: ['Nuwara Eliya', 'Hatton', 'Nawalapitiya', 'Bandarawela'],
      },
    ],
  },
  {
    name: 'Southern Province',
    districts: [
      {
        name: 'Galle',
        areas: ['Galle City', 'Hikkaduwa', 'Ambalangoda', 'Unawatuna', 'Elpitiya'],
      },
      {
        name: 'Matara',
        areas: ['Matara', 'Weligama', 'Mirissa', 'Akuressa', 'Hakmana'],
      },
      {
        name: 'Hambantota',
        areas: ['Hambantota', 'Tangalle', 'Tissamaharama', 'Ambalantota'],
      },
    ],
  },
  {
    name: 'Northern Province',
    districts: [
      { name: 'Jaffna', areas: ['Jaffna Town', 'Nallur', 'Chavakachcheri', 'Point Pedro'] },
      { name: 'Kilinochchi', areas: ['Kilinochchi', 'Pallai'] },
      { name: 'Mannar', areas: ['Mannar', 'Pesalai'] },
      { name: 'Vavuniya', areas: ['Vavuniya', 'Nedunkeni'] },
      { name: 'Mullaitivu', areas: ['Mullaitivu', 'Puthukudiyiruppu'] },
    ],
  },
  {
    name: 'Eastern Province',
    districts: [
      { name: 'Batticaloa', areas: ['Batticaloa', 'Kattankudy', 'Eravur'] },
      { name: 'Ampara', areas: ['Ampara', 'Kalmunai', 'Sainthamaruthu'] },
      { name: 'Trincomalee', areas: ['Trincomalee', 'Kinniya', 'Mutur'] },
    ],
  },
  {
    name: 'North Western Province',
    districts: [
      { name: 'Kurunegala', areas: ['Kurunegala', 'Kuliyapitiya', 'Narammala', 'Polgahawela'] },
      { name: 'Puttalam', areas: ['Puttalam', 'Chilaw', 'Wennappuwa'] },
    ],
  },
  {
    name: 'North Central Province',
    districts: [
      { name: 'Anuradhapura', areas: ['Anuradhapura', 'Medawachchiya', 'Kekirawa'] },
      { name: 'Polonnaruwa', areas: ['Polonnaruwa', 'Hingurakgoda', 'Medirigiriya'] },
    ],
  },
  {
    name: 'Uva Province',
    districts: [
      { name: 'Badulla', areas: ['Badulla', 'Bandarawela', 'Hali-Ela', 'Passara'] },
      { name: 'Monaragala', areas: ['Monaragala', 'Wellawaya', 'Bibile'] },
    ],
  },
  {
    name: 'Sabaragamuwa Province',
    districts: [
      { name: 'Ratnapura', areas: ['Ratnapura', 'Balangoda', 'Embilipitiya', 'Kuruwita'] },
      { name: 'Kegalle', areas: ['Kegalle', 'Mawanella', 'Warakapola', 'Rambukkana'] },
    ],
  },
];
