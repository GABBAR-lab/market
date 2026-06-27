import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'll_saved_listings';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly _ids = signal<Set<string>>(this.load());

  readonly count = computed(() => this._ids().size);

  isSaved(id: string): boolean {
    return this._ids().has(id);
  }

  toggle(id: string): boolean {
    const next = new Set(this._ids());
    const added = !next.has(id);
    if (added) {
      next.add(id);
    } else {
      next.delete(id);
    }
    this._ids.set(next);
    this.persist(next);
    return added;
  }

  getAll(): string[] {
    return [...this._ids()];
  }

  private load(): Set<string> {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return new Set(raw ? (JSON.parse(raw) as string[]) : []);
    } catch {
      return new Set();
    }
  }

  private persist(ids: Set<string>): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify([...ids]));
  }
}
