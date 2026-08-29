import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./shared/layout/layout.component').then(m => m.LayoutComponent),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'unidades' },
      {
        path: 'usuarios',
        loadComponent: () => import('./features/usuarios/usuarios.component').then(m => m.UsuariosComponent)
      },
      {
        path: 'colaboradores',
        loadComponent: () => import('./features/colaboradores/colaboradores.component').then(m => m.ColaboradoresComponent)
      },
      {
        path: 'unidades',
        loadComponent: () => import('./features/unidades/unidades.component').then(m => m.UnidadesComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'unidades' }
];
