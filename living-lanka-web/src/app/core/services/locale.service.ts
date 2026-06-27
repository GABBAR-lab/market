import { Injectable, signal } from '@angular/core';

export type AppLanguage = 'en' | 'si' | 'ta';

@Injectable({ providedIn: 'root' })
export class LocaleService {
  private readonly _lang = signal<AppLanguage>(
    (localStorage.getItem('ll_lang') as AppLanguage) ?? 'en'
  );

  readonly lang = this._lang.asReadonly();

  setLanguage(lang: AppLanguage): void {
    this._lang.set(lang);
    localStorage.setItem('ll_lang', lang);
  }

  label(key: string): string {
    const lang = this._lang();
    const map: Record<AppLanguage, Record<string, string>> = {
      en: {
        allAds: 'All ads',
        postAd: 'POST YOUR AD',
        login: 'Login',
        chat: 'Chat',
        featured: 'Featured Ads',
        browse: 'Browse items by category',
      },
      si: {
        allAds: 'සියලුම දැන්වීම්',
        postAd: 'දැන්වීමක් දමන්න',
        login: 'පිවිසෙන්න',
        chat: 'කතාබහ',
        featured: 'විශේෂ දැන්වීම්',
        browse: 'කාණ්ඩ අනුව බලන්න',
      },
      ta: {
        allAds: 'அனைத்து விளம்பரங்கள்',
        postAd: 'விளம்பரம் இடுக',
        login: 'உள்நுழை',
        chat: 'அரட்டை',
        featured: 'சிறப்பு விளம்பரங்கள்',
        browse: 'வகையின் அடிப்படையில் உலாவு',
      },
    };
    return map[lang][key] ?? map.en[key] ?? key;
  }
}
