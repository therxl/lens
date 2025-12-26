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

  constructor(private http: HttpClient, private auth: AuthService) {
    // No need to load favorites here, will load from backend
  }

  getLenses(): Observable<Lens[]> {
    return this.http.get<Lens[]>('http://localhost:5053/api/lenses');
  }

  getLensById(id: number): Observable<Lens | undefined> {
    return this.http.get<Lens>(`http://localhost:5053/api/lenses/${id}`);
  }

  addToFavorites(lensId: number): Observable<any> {
    const userId = this.auth.userId;
    return this.http.post(`http://localhost:5053/api/favorites?userId=${userId}`, lensId);
  }

  getFavorites(): Observable<Lens[]> {
    const userId = this.auth.userId;
    return this.http.get<Lens[]>(`http://localhost:5053/api/favorites?userId=${userId}`);
  }

  removeFromFavorites(lensId: number): Observable<any> {
    const userId = this.auth.userId;
    return this.http.delete(`http://localhost:5053/api/favorites?userId=${userId}&lensId=${lensId}`);
  }
}
