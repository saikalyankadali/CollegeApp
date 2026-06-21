import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="card">
      <h2>Login</h2>
      <input [(ngModel)]="userName" placeholder="Username" />
      <input [(ngModel)]="password" type="password" placeholder="Password" />
      <button class="btn-primary" (click)="onLogin()" [disabled]="loading">
        {{ loading ? 'Signing in...' : 'Login' }}
      </button>
      @if (error) {
        <p class="error">{{ error }}</p>
      }
    </div>
  `,
  styles: [
    `
      .card {
        max-width: 360px;
        margin: 80px auto;
        padding: 32px;
        background: var(--card);
        border-radius: 14px;
        box-shadow: 0 10px 30px rgba(15, 23, 42, 0.08);
        display: flex;
        flex-direction: column;
        gap: 14px;
      }
      .card h2 {
        margin: 0 0 6px;
        text-align: center;
        color: var(--navy);
      }
      .error {
        color: var(--danger);
        font-size: 13px;
        margin: 0;
        text-align: center;
      }
    `,
  ],
})
export class LoginComponent {
  userName = '';
  password = '';
  error = '';
  loading = false;

  constructor(
    private auth: AuthService,
    private router: Router,
  ) {}

  onLogin() {
    this.error = '';
    this.loading = true;
    this.auth.login(this.userName, this.password).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.status) {
          this.router.navigate(['/students']);
        } else {
          // API returns 200 with status:false for bad credentials
          this.error = res.errors?.join(', ') || 'Invalid credentials';
        }
      },
      error: () => {
        this.loading = false;
        this.error = 'Login failed — server unreachable';
      },
    });
  }
}
