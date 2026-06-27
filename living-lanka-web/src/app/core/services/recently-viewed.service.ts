import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'll_recently_viewed';
const MAX_ITEMS = 12;

@Injectable({ providedIn: 'root' })
export class RecentlyViewedService {
  private readonly _ids = signal<string[]>(this.load());

  readonly ids = this._ids.asReadonly();

  add(id: string): void {
    const next = [id, ...this._ids().filter((x) => x !== id)].slice(0, MAX_ITEMS);
    this._ids.set(next);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
  }

  getAll(): string[] {
    return [...this._ids()];
  }

  private load(): string[] {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as string[]) : [];
    } catch {
      return [];
    }
  }
}
