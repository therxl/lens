import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  username = '';
  password = '';
  errorMessage = '';

  constructor(
    private router: Router,
    private auth: AuthService,
    private http: HttpClient
  ) {}

  login() {
    if (!this.username || !this.password) {
      this.errorMessage = 'Введите логин и пароль';
      return;
    }

    this.http.post<any>('http://localhost:5053/api/auth/login', { username: this.username, password: this.password })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.errorMessage = '';
            this.auth.loginAsUser(this.username, response.userId);
            this.router.navigate(['/home']);
          } else {
            this.errorMessage = 'Неверный логин или пароль';
          }
        },
        error: () => {
          this.errorMessage = 'Ошибка сервера';
        }
      });
  }

  continueAsGuest() {
    this.errorMessage = '';
    this.http.post<any>('http://localhost:5053/api/auth/guest', {})
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.auth.loginAsGuest(response.userId);
            this.router.navigate(['/home']);
          } else {
            this.errorMessage = 'Ошибка';
          }
        },
        error: () => {
          this.errorMessage = 'Ошибка сервера';
        }
      });
  }
}
