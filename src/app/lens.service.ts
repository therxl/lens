import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface Lens {
  id: number;
  name: string;
  type: string;
  focalLength: string;
  minFocal: number;
  maxFocal: number;
  aperture: string;
  compatibility: string;
  brand: string;
  price: number;
  description: string;
  isPopular?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class LensService {
  private apiUrl = 'http://localhost:5126/api/lenses';

  private favorites: Lens[] = [];

  constructor(private http: HttpClient, private auth: AuthService) {
    this.loadFavorites();
  }

  // Ключ для localStorage для гостя и пользователя
  private get storageKey(): string {
    // если в AuthService есть getter currentKey — используй его
    if ((this.auth as any).currentKey) {
      return (this.auth as any).currentKey;
    }

    // иначе просто разделяем guest / user по mode
    const mode = this.auth.mode; // ВАЖНО: без (), это не функция
    if (mode === 'user') {
      return 'favorites_user';
    }
    return 'favorites_guest';
  }

  private loadFavorites(): void {
    const saved = localStorage.getItem(this.storageKey);
    this.favorites = saved ? JSON.parse(saved) : [];
  }

  private saveFavorites(): void {
    localStorage.setItem(this.storageKey, JSON.stringify(this.favorites));
  }

  // вызывать после логина/логаута
  refreshFavoritesForCurrentUser(): void {
    this.loadFavorites();
  }

  getLenses(): Observable<Lens[]> {
    return this.http.get<Lens[]>(this.apiUrl);
  }

  getLensById(id: number): Observable<Lens | undefined> {
    return this.http.get<Lens>(`${this.apiUrl}/${id}`);
  }

  addToFavorites(lens: Lens): void {
    if (!this.favorites.some(f => f.id === lens.id)) {
      this.favorites.push(lens);
      this.saveFavorites();
    }
  }

  getFavorites(): Lens[] {
    return this.favorites;
  }

  removeFromFavorites(lens: Lens): void {
    this.favorites = this.favorites.filter(f => f.id !== lens.id);
    this.saveFavorites();
  }
}
