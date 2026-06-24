import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RoleService } from '../../services/role.service';
import { RolePrivilegeService } from '../../services/role-privilege.service';
import { Role, RoleDto } from '../../models/role.model';
import { RolePrivilege, RolePrivilegeDto } from '../../models/role-privilege.model';

@Component({
  selector: 'app-role-management',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h2>Role Management</h2>

    <div class="create-box">
      <h3>Add Role</h3>
      <div class="row">
        <input [(ngModel)]="newRole.roleName" placeholder="Role name" />
        <input [(ngModel)]="newRole.description" placeholder="Description" />
        <button class="btn-primary" (click)="create()" [disabled]="saving">Add</button>
      </div>
      @if (createError) { <p class="error">{{ createError }}</p> }
    </div>

    @if (error) { <p class="error">{{ error }}</p> }

    <table>
      <thead>
        <tr><th>Id</th><th>Role</th><th>Description</th><th>Actions</th></tr>
      </thead>
      <tbody>
        @for (r of roles; track r.id) {
          <tr>
            <td>{{ r.id }}</td>
            @if (editing?.id === r.id) {
              <td><input [(ngModel)]="editing!.roleName" /></td>
              <td><input [(ngModel)]="editing!.description" /></td>
              <td>
                <button class="btn-success btn-sm" (click)="saveEdit()">Save</button>
                <button class="btn-ghost btn-sm" (click)="editing = null">Cancel</button>
              </td>
            } @else {
              <td>{{ r.roleName }}</td>
              <td>{{ r.description }}</td>
              <td>
                <button class="btn-ghost btn-sm" (click)="startEdit(r)">Edit</button>
                <button class="btn-danger btn-sm" (click)="remove(r.id)">Delete</button>
                <button class="btn-ghost btn-sm" (click)="togglePrivileges(r.id)">
                  {{ expandedRoleId === r.id ? 'Hide' : 'Privileges' }}
                </button>
              </td>
            }
          </tr>
          @if (expandedRoleId === r.id) {
            <tr class="priv-row">
              <td colspan="4">
                <div class="priv-panel">
                  <strong>Privileges for {{ r.roleName }}</strong>

                  <div class="add-priv">
                    <input [(ngModel)]="newPriv.rolePrivilegeName" placeholder="Privilege name" />
                    <input [(ngModel)]="newPriv.description" placeholder="Description" />
                    <label class="chk">
                      <input type="checkbox" [(ngModel)]="newPriv.isActive" /> Is Active
                    </label>
                    <button class="btn-primary btn-sm" (click)="addPrivilege(r.id)">Add</button>
                  </div>

                  @if (privileges.length === 0) {
                    <p class="muted">No privileges assigned.</p>
                  } @else {
                    <table class="inner">
                      <thead>
                        <tr><th>Name</th><th>Description</th><th>Active</th><th></th></tr>
                      </thead>
                      <tbody>
                        @for (p of privileges; track p.id) {
                          <tr>
                            <td>{{ p.rolePrivilegeName }}</td>
                            <td>{{ p.description }}</td>
                            <td>{{ p.isActive ? 'Yes' : 'No' }}</td>
                            <td>
                              <button class="btn-danger btn-sm" (click)="removePrivilege(p.id, r.id)">Remove</button>
                            </td>
                          </tr>
                        }
                      </tbody>
                    </table>
                  }
                </div>
              </td>
            </tr>
          }
        }
      </tbody>
    </table>
  `,
  styles: [`
    :host { display:block; max-width:1000px; margin:32px auto; padding:0 20px; }
    h2 { color: var(--navy); }
    .create-box { margin:20px 0; padding:18px; background:var(--card); border:1px solid var(--border); border-radius:12px; }
    .create-box h3 { margin:0 0 12px; color:var(--steel); font-size:16px; }
    .create-box .row { display:flex; gap:10px; flex-wrap:wrap; }
    .create-box input { flex:1; min-width:150px; }
    table { width:100%; border-collapse:separate; border-spacing:0; background:var(--card); border-radius:12px; overflow:hidden; box-shadow:0 4px 16px rgba(15,23,42,.05); }
    thead th { background:var(--navy); color:#fff; text-align:left; padding:12px 14px; font-size:13px; text-transform:uppercase; letter-spacing:.03em; }
    tbody td { padding:11px 14px; border-bottom:1px solid var(--border); font-size:14px; }
    tbody tr:hover { background:#f8fafc; }
    .priv-row td { background:#f8fafc; }
    .priv-panel { padding:10px 4px; }
    .add-priv { display:flex; gap:10px; align-items:center; margin:10px 0; flex-wrap:wrap; }
    .add-priv input { min-width:160px; }
    .chk { display:flex; align-items:center; gap:6px; font-size:14px; color:var(--steel); white-space:nowrap; }
    .chk input { width:auto; }
    table.inner { box-shadow:none; border:1px solid var(--border); }
    table.inner thead th { background:var(--steel); padding:8px 12px; }
    table.inner tbody td { padding:8px 12px; }
    .muted { color:var(--muted); font-size:13px; }
    .error { color:var(--danger); }
  `]
})
export class RoleManagementComponent implements OnInit {
  roles: Role[] = [];
  editing: Role | null = null;
  newRole: RoleDto = { roleName: '', description: '' };
  expandedRoleId: number | null = null;
  privileges: RolePrivilege[] = [];
  newPriv: RolePrivilegeDto = this.emptyPriv();
  saving = false;
  error = '';
  createError = '';

  constructor(private roleService: RoleService, private privService: RolePrivilegeService) {}

  ngOnInit() { this.load(); }

  emptyPriv(): RolePrivilegeDto {
    return { roleId: 0, rolePrivilegeName: '', description: '', isActive: true };
  }

  load() {
    this.roleService.getAll().subscribe({
      next: (data) => this.roles = data,
      error: () => this.error = 'Failed to load roles'
    });
  }

  create() {
    this.createError = ''; this.saving = true;
    this.roleService.create(this.newRole).subscribe({
      next: () => { this.newRole = { roleName: '', description: '' }; this.saving = false; this.load(); },
      error: () => { this.createError = 'Create failed'; this.saving = false; }
    });
  }

  startEdit(r: Role) { this.editing = { ...r }; }
  saveEdit() {
    if (!this.editing) return;
    this.roleService.update(this.editing).subscribe({
      next: (res) => res.status ? (this.editing = null, this.load()) : this.error = res.errors?.join(', ') || 'Update failed',
      error: () => this.error = 'Update failed'
    });
  }
  remove(id: number) {
    if (!confirm('Delete this role?')) return;
    this.roleService.delete(id).subscribe({
      next: (res) => res.status ? this.load() : this.error = 'Delete failed',
      error: () => this.error = 'Delete failed'
    });
  }

  togglePrivileges(roleId: number) {
    if (this.expandedRoleId === roleId) { this.expandedRoleId = null; return; }
    this.expandedRoleId = roleId;
    this.newPriv = this.emptyPriv();   // reset form when opening a different role
    this.loadPrivileges(roleId);
  }
  loadPrivileges(roleId: number) {
    this.privService.getByRole(roleId).subscribe({
      next: (data) => this.privileges = data,
      error: () => this.privileges = []
    });
  }
  addPrivilege(roleId: number) {
    if (!this.newPriv.rolePrivilegeName.trim()) return;
    // RoleId taken from the role whose panel we're adding under
    const payload: RolePrivilegeDto = { ...this.newPriv, roleId };
    this.privService.add(payload).subscribe({
      next: () => { this.newPriv = this.emptyPriv(); this.loadPrivileges(roleId); },
      error: () => this.error = 'Add privilege failed'
    });
  }
  removePrivilege(privId: number, roleId: number) {
    this.privService.remove(privId).subscribe({
      next: () => this.loadPrivileges(roleId),
      error: () => this.error = 'Remove privilege failed'
    });
  }
}