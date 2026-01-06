import { Component } from '@angular/core';
import { Router } from '@angular/router';
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
    private auth: AuthService
  ) {}

  login() {
    if (!this.username || !this.password) {
      this.errorMessage = 'Введите логин и пароль';
      return;
    }

    // фейковая авторизация
    if (this.username === 'user' && this.password === '1234') {
      this.errorMessage = '';
      this.auth.loginAsUser(this.username);
      this.router.navigate(['/home']);
    } else {
      this.errorMessage = 'Неверный логин или пароль';
    }
  }

  continueAsGuest() {
    this.errorMessage = '';
    this.auth.loginAsGuest();
    this.router.navigate(['/home']);
  }
}
