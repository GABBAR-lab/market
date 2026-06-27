import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, tap, catchError, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

const STORAGE_KEY = 'll_saved_listings';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly base = `${environment.apiBaseUrl}/favorites`;
  private readonly _ids = signal<Set<string>>(this.loadFromStorage());

  readonly count = computed(() => this._ids().size);

  constructor() {
    if (this.auth.isLoggedIn()) {
      this.syncFromServer();
    }
  }

  isSaved(id: string): boolean {
    return this._ids().has(id);
  }

  toggle(id: string): Observable<boolean> {
    const added = !this._ids().has(id);
    this.updateLocal(id, added);

    if (!this.auth.isLoggedIn()) {
      return of(added);
    }

    const req = added
      ? this.http.post<{ message: string }>(`${this.base}/${id}`, {})
      : this.http.delete<{ message: string }>(`${this.base}/${id}`);

    return req.pipe(
      map(() => added),
      catchError(() => {
        this.updateLocal(id, !added);
        throw new Error('Failed to sync favorite');
      })
    );
  }

  getAll(): string[] {
    return [...this._ids()];
  }

  syncFromServer(): void {
    if (!this.auth.isLoggedIn()) return;
    this.http.get<string[]>(this.base).pipe(
      tap((ids) => {
        this._ids.set(new Set(ids));
        this.persistToStorage(ids);
      }),
      catchError(() => of([]))
    ).subscribe();
  }

  private updateLocal(id: string, added: boolean): void {
    const next = new Set(this._ids());
    if (added) next.add(id);
    else next.delete(id);
    this._ids.set(next);
    this.persistToStorage([...next]);
  }

  private loadFromStorage(): Set<string> {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return new Set(raw ? (JSON.parse(raw) as string[]) : []);
    } catch {
      return new Set();
    }
  }

  private persistToStorage(ids: string[]): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
  }
}
