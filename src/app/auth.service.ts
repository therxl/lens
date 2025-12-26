import { Injectable } from '@angular/core';

export type UserMode = 'guest' | 'user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _isLoggedIn = false;
  private _mode: UserMode | null = null;
  private _username: string | null = null;
  private _userId: string | null = null;

  get isLoggedIn(): boolean {
    return this._isLoggedIn;
  }

  get mode(): UserMode | null {
    return this._mode;
  }

  get userId(): string | null {
    return this._userId;
  }

  get currentKey(): string {
    // ключ для localStorage (разное избранное для гостя и юзера)
    if (!this._isLoggedIn || !this._mode) {
      return 'favorites_guest';
    }
    if (this._mode === 'guest') {
      return 'favorites_guest';
    }
    // для простоты — один пользователь с логином user
    return `favorites_user`;
  }

  loginAsUser(username: string, userId: string): void {
    this._isLoggedIn = true;
    this._mode = 'user';
    this._username = username;
    this._userId = userId;
  }

  loginAsGuest(userId: string): void {
    this._isLoggedIn = true;
    this._mode = 'guest';
    this._username = null;
    this._userId = userId;
  }

  logout(): void {
    this._isLoggedIn = false;
    this._mode = null;
    this._username = null;
  }
}
