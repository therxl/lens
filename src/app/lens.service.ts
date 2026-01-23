import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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
  private baseUrl = 'http://localhost:5093/api';

  constructor(private http: HttpClient, private auth: AuthService) {}

  private getUserId(): string {
    const mode = this.auth.mode;
    return mode === 'user' ? 'user' : 'guest';
  }

  getLenses(): Observable<Lens[]> {
    return this.http.get<Lens[]>(`${this.baseUrl}/lenses`);
  }

  getLensById(id: number): Observable<Lens> {
    return this.http.get<Lens>(`${this.baseUrl}/lenses/${id}`);
  }

  getFavorites(): Observable<Lens[]> {
    const userId = this.getUserId();
    return this.http.get<Lens[]>(`${this.baseUrl}/favorites?userId=${userId}`);
  }

  addToFavorites(lens: Lens): Observable<any> {
    const userId = this.getUserId();
    return this.http.post(`${this.baseUrl}/favorites?userId=${userId}`, lens);
  }

  removeFromFavorites(lens: Lens): Observable<any> {
    const userId = this.getUserId();
    return this.http.delete(`${this.baseUrl}/favorites/${lens.id}?userId=${userId}`);
  }

  // For compatibility, but now async
  refreshFavoritesForCurrentUser(): void {
    // No-op, since data is fetched on demand
  }
}
