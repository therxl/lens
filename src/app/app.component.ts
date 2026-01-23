import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'photomag';

  constructor(public auth: AuthService) {}

  ngOnInit(): void {
    // Set default mode to guest
    this.auth.loginAsGuest();
  }
}
