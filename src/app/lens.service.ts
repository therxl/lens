import { Injectable } from '@angular/core';
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
  private lenses: Lens[] = [
    {
      id: 1,
      name: 'Canon EF 85mm f/1.8',
      type: 'portrait',
      focalLength: '85mm',
      minFocal: 85,
      maxFocal: 85,
      aperture: 'f/1.8',
      compatibility: 'Canon EF',
      brand: 'Canon',
      price: 35000,
      description: 'Классический портретный объектив.',
      isPopular: true
    },
    {
      id: 2,
      name: 'Canon EF 24-70mm f/2.8',
      type: 'landscape',
      focalLength: '24–70mm',
      minFocal: 24,
      maxFocal: 70,
      aperture: 'f/2.8',
      compatibility: 'Canon EF',
      brand: 'Canon',
      price: 90000,
      description: 'Универсальный зум для пейзажей и репортажей.',
      isPopular: true
    },
    {
      id: 3,
      name: 'Nikon AF-S 70-200mm f/2.8',
      type: 'sport',
      focalLength: '70–200mm',
      minFocal: 70,
      maxFocal: 200,
      aperture: 'f/2.8',
      compatibility: 'Nikon F',
      brand: 'Nikon',
      price: 130000,
      description: 'Телезум для спорта и съёмки с расстояния.',
      isPopular: true
    },
    {
      id: 4,
      name: 'Sony FE 90mm f/2.8 Macro',
      type: 'macro',
      focalLength: '90mm',
      minFocal: 90,
      maxFocal: 90,
      aperture: 'f/2.8',
      compatibility: 'Sony FE',
      brand: 'Sony',
      price: 110000,
      description: 'Макрообъектив для съёмки мелких деталей.'
    },
    {
      id: 5,
      name: 'Sigma 16mm f/1.4 DC DN',
      type: 'landscape',
      focalLength: '16mm',
      minFocal: 16,
      maxFocal: 16,
      aperture: 'f/1.4',
      compatibility: 'Sony E / m4/3',
      brand: 'Sigma',
      price: 45000,
      description: 'Широкоугольный объектив для пейзажей и интерьеров.'
    }
  ];

  private favorites: Lens[] = [];

  constructor(private auth: AuthService) {
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

  getLenses(): Lens[] {
    return this.lenses;
  }

  getLensById(id: number): Lens | undefined {
    return this.lenses.find(l => l.id === id);
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
