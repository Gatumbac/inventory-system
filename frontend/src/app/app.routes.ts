import { Routes } from '@angular/router';
import { adminGuard } from './core/auth/admin.guard';
import { authGuard } from './core/auth/auth.guard';
import { Shell } from './core/layout/shell';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/login/login').then((m) => m.Login) },
  {
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'products' },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/products/product-list/product-list').then((m) => m.ProductList),
      },
      {
        path: 'products/new',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/products/product-form/product-form').then((m) => m.ProductForm),
      },
      {
        path: 'products/:id/edit',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/products/product-form/product-form').then((m) => m.ProductForm),
      },
      {
        path: 'suppliers',
        loadComponent: () =>
          import('./features/suppliers/supplier-list/supplier-list').then((m) => m.SupplierList),
      },
      {
        path: 'suppliers/new',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/suppliers/supplier-form/supplier-form').then((m) => m.SupplierForm),
      },
      {
        path: 'suppliers/:id/edit',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/suppliers/supplier-form/supplier-form').then((m) => m.SupplierForm),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
