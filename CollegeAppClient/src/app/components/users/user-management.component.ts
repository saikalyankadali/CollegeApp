import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { UserService } from '../../services/user.service';
import { UserType } from '../../models/user-type.model';
import { UserTypeService } from '../../services/user-type.service';
import { User, UserDto } from '../../models/user.model';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [FormsModule, DatePipe],
  template: `
    <h2>User Management</h2>

    <div class="create-box">
      <h3>Add User</h3>
      <div class="row">
        <input [(ngModel)]="newUser.username" placeholder="Username" />
        <input [(ngModel)]="newUser.password" type="password" placeholder="Password" />
        <input [(ngModel)]="newUser.userTypeId" type="number" placeholder="User Type Id" />
        <label class="chk"><input type="checkbox" [(ngModel)]="newUser.isActive" /> Is Active</label>
        <label class="chk"><input type="checkbox" [(ngModel)]="newUser.isDeleted" /> Is Deleted</label>
        <button class="btn-primary" (click)="create()" [disabled]="saving">Add</button>
      </div>
      @if (createError) { <p class="error">{{ createError }}</p> }
    </div>

    @if (error) { <p class="error">{{ error }}</p> }

    <table>
      <thead>
        <tr><th>Username</th><th>Is Active</th><th>User Type Id</th><th>Actions</th></tr>
      </thead>
      <tbody>
        @for (u of users; track u.id) {
          <tr>
            <td>{{ u.username }}</td>
            <td>{{ u.isActive ? 'Yes' : 'No' }}</td>
            <td>{{ u.userTypeId }}</td>
            <td>
              <button class="btn-danger btn-sm" (click)="remove(u.id)">Delete</button>
            </td>
          </tr>
        }
      </tbody>
    </table>
  `,
  styles: [`
    :host { display:block; max-width:1000px; margin:32px auto; padding:0 20px; }
    h2 { color:var(--navy); }
    .create-box { margin:20px 0; padding:18px; background:var(--card); border:1px solid var(--border); border-radius:12px; }
    .create-box h3 { margin:0 0 12px; color:var(--steel); font-size:16px; }
    .create-box .row { display:flex; gap:12px; flex-wrap:wrap; align-items:center; }
    .create-box input[type="text"], .create-box input[type="password"], .create-box input[type="number"] { flex:1; min-width:140px; }
    .chk { display:flex; align-items:center; gap:6px; font-size:14px; color:var(--steel); white-space:nowrap; }
    .chk input { width:auto; }
    select { padding:9px 11px; border:1px solid var(--border); border-radius:8px; }
    table { width:100%; border-collapse:separate; border-spacing:0; background:var(--card); border-radius:12px; overflow:hidden; box-shadow:0 4px 16px rgba(15,23,42,.05); }
    thead th { background:var(--navy); color:#fff; text-align:left; padding:12px 14px; font-size:13px; text-transform:uppercase; letter-spacing:.03em; }
    tbody td { padding:11px 14px; border-bottom:1px solid var(--border); font-size:14px; }
    tbody tr:hover { background:#f8fafc; }
    .error { color:var(--danger); }
  `]
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  userTypes: UserType[] = [];
  newUser: UserDto = this.emptyUser();
  saving = false;
  error = '';
  createError = '';

  constructor(
    private userService: UserService,
    private userTypeService: UserTypeService
  ) {}

  ngOnInit() {
    this.userTypeService.getAll().subscribe({ next: (d) => this.userTypes = d });
    this.load();
  }

  emptyUser(): UserDto {
    return { username: '', password: '', userTypeId: undefined, isActive: true, isDeleted: false };
  }

  load() {
    this.userService.getAll().subscribe({
      next: (d) => this.users = d,
      error: () => this.error = 'Failed to load users'
    });
  }

  create() {
    this.createError = ''; this.saving = true;
    this.userService.create(this.newUser).subscribe({
      next: () => { this.newUser = this.emptyUser(); this.saving = false; this.load(); },
      error: () => { this.createError = 'Create failed'; this.saving = false; }
    });
  }

  remove(id: number) {
    if (!confirm('Delete this user?')) return;
    this.userService.delete(id).subscribe({
      next: (res) => res.status ? this.load() : this.error = 'Delete failed',
      error: () => this.error = 'Delete failed'
    });
  }
}