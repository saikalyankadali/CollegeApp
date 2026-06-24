import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    @if (auth.isLoggedIn()) {
      <nav class="topnav">
        <a routerLink="/students">Students</a>
        <a routerLink="/roles">Roles</a>
        <a routerLink="/users">Users</a>
      </nav>
    }
    <router-outlet />
  `,
  styles: [`
    .topnav { display:flex; gap:18px; padding:14px 24px; background:var(--navy); }
    .topnav a { color:#fff; text-decoration:none; font-weight:600; opacity:.85; }
    .topnav a:hover { opacity:1; }
  `]
})
export class AppComponent {
  constructor(public auth: AuthService) {}
}