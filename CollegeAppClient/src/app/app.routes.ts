import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./components/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'students',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./components/student-list/student-list.component').then(
        (m) => m.StudentListComponent
      )
  }
];