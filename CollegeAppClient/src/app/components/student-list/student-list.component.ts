import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { StudentService } from '../../services/student.service';
import { AuthService } from '../../services/auth.service';
import { Student, StudentDto } from '../../models/student.model';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [FormsModule, DatePipe],
  template: `
    <div class="header">
      <h2>Students</h2>
      <span>{{ auth.userName() }}</span>
      <button class="btn-ghost btn-sm" (click)="logout()">Logout</button>
    </div>

    <!-- CREATE (POST) -->
    <div class="create-box">
      <h3>Add Student</h3>
      <div class="row">
        <input [(ngModel)]="newStudent.studentName" placeholder="Name" />
        <input [(ngModel)]="newStudent.email" placeholder="Email" />
        <input [(ngModel)]="newStudent.phone" placeholder="Phone" />
        <input type="date" [(ngModel)]="newStudent.dob" />
        <input
          type="number"
          [(ngModel)]="newStudent.departmentId"
          placeholder="Dept Id"
        />
        <button class="btn-primary" (click)="create()" [disabled]="saving">
          {{ saving ? 'Adding...' : 'Add' }}
        </button>
      </div>
      @if (createError) {
        <p class="error">{{ createError }}</p>
      }
    </div>

    @if (loading) {
      <p>Loading...</p>
    }
    @if (error) {
      <p class="error">{{ error }}</p>
    }

    <table>
      <thead>
        <tr>
          <th>Id</th>
          <th>Name</th>
          <th>Email</th>
          <th>Phone</th>
          <th>DOB</th>
          <th>Dept</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        @for (s of students; track s.id) {
          <tr>
            <td>{{ s.id }}</td>
            @if (editing?.id === s.id) {
              <td><input [(ngModel)]="editing!.studentName" /></td>
              <td><input [(ngModel)]="editing!.email" /></td>
              <td><input [(ngModel)]="editing!.phone" /></td>
              <td><input type="date" [(ngModel)]="editing!.dob" /></td>
              <td>
                <input type="number" [(ngModel)]="editing!.departmentId" />
              </td>
              <td>
                <button class="btn-success btn-sm" (click)="saveEdit()">
                  Save
                </button>
                <button class="btn-ghost btn-sm" (click)="editing = null">
                  Cancel
                </button>
              </td>
            } @else {
              <td>{{ s.studentName }}</td>
              <td>{{ s.email }}</td>
              <td>{{ s.phone }}</td>
              <td>{{ s.dob | date: 'yyyy-MM-dd' }}</td>
              <td>{{ s.departmentId }}</td>
              <td>
                <button class="btn-ghost btn-sm" (click)="startEdit(s)">
                  Edit
                </button>
                <button class="btn-danger btn-sm" (click)="remove(s.id)">
                  Delete
                </button>
              </td>
            }
          </tr>
        }
      </tbody>
    </table>
  `,
  styles: [
    `
      :host {
        display: block;
        max-width: 1100px;
        margin: 32px auto;
        padding: 0 20px;
      }
      .header {
        display: flex;
        gap: 14px;
        align-items: center;
        padding-bottom: 16px;
        border-bottom: 2px solid var(--border);
      }
      .header h2 {
        margin: 0 auto 0 0;
        color: var(--navy);
      }
      .header span {
        color: var(--muted);
        font-weight: 600;
      }

      .create-box {
        margin: 24px 0;
        padding: 20px;
        background: var(--card);
        border: 1px solid var(--border);
        border-radius: 12px;
        box-shadow: 0 4px 12px rgba(15, 23, 42, 0.04);
      }
      .create-box h3 {
        margin: 0 0 14px;
        color: var(--steel);
        font-size: 16px;
      }
      .create-box .row {
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
        align-items: center;
      }
      .create-box .row input {
        flex: 1;
        min-width: 130px;
      }

      table {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0;
        background: var(--card);
        border-radius: 12px;
        overflow: hidden;
        box-shadow: 0 4px 16px rgba(15, 23, 42, 0.05);
      }
      thead th {
        background: var(--navy);
        color: #fff;
        text-align: left;
        padding: 13px 14px;
        font-size: 13px;
        text-transform: uppercase;
        letter-spacing: 0.03em;
      }
      tbody td {
        padding: 11px 14px;
        border-bottom: 1px solid var(--border);
        font-size: 14px;
      }
      tbody tr:last-child td {
        border-bottom: none;
      }
      tbody tr:hover {
        background: #f8fafc;
      }
      td input {
        width: 100%;
      }

      .error {
        color: var(--danger);
        font-size: 14px;
      }
    `,
  ],
})
export class StudentListComponent implements OnInit {
  students: Student[] = [];
  editing: Student | null = null;
  newStudent: StudentDto = this.emptyStudent();
  loading = false;
  saving = false;
  error = '';
  createError = '';

  constructor(
    private studentService: StudentService,
    public auth: AuthService,
    private router: Router,
  ) {}

  ngOnInit() {
    this.load();
  }

  emptyStudent(): StudentDto {
    return {
      studentName: '',
      email: '',
      phone: '',
      dob: '',
      departmentId: null,
    };
  }

  load() {
    this.loading = true;
    this.error = '';
    this.studentService.getAll().subscribe({
      next: (data) => {
        this.students = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load students';
        this.loading = false;
      },
    });
  }

  create() {
    this.createError = '';
    this.saving = true;
    this.studentService.create(this.newStudent).subscribe({
      next: () => {
        this.newStudent = this.emptyStudent();
        this.saving = false;
        this.load();
      },
      error: (err) => {
        this.createError =
          err.status === 400
            ? 'Validation failed — check fields'
            : 'Create failed';
        this.saving = false;
      },
    });
  }

  saveEdit() {
    if (!this.editing) return;
    this.studentService.update(this.editing).subscribe({
      next: (res) => {
        if (res.status) {
          this.editing = null;
          this.load();
        } else {
          this.error = res.errors?.join(', ') || 'Update failed';
        }
      },
      error: () => (this.error = 'Update failed'),
    });
  }

  remove(id: number) {
    if (!confirm('Delete this student?')) return;
    this.studentService.delete(id).subscribe({
      next: (deleted) => {
        if (deleted) this.load();
        else this.error = 'Delete failed';
      },
      error: () => (this.error = 'Delete failed'),
    });
  }

  startEdit(s: Student) {
    this.editing = { ...s, dob: s.dob ? s.dob.substring(0, 10) : '' };
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
