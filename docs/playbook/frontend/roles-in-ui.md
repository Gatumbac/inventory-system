# Roles in the UI

The backend decides. The UI only hides what the user cannot use, so they do not hit a 403.

## How it works

- At login, `AuthService.login` stores `token`, `role` and `email` in `localStorage`.
- `AuthService.isAdmin()` → `role() === 'Admin'`.
- `authGuard` (any logged-in user) protects the shell. `adminGuard` protects create/edit routes and sends a Viewer to `/products`.
- Lists and the detail page compute `isAdmin` once and use it for buttons and columns.

## Hide an action

```ts
protected readonly isAdmin = inject(AuthService).isAdmin();
protected readonly columns = this.isAdmin
  ? ['name', 'description', 'actions']
  : ['name', 'description'];
```

```html
@if (isAdmin) {
  <a mat-flat-button routerLink="/suppliers/new">Nuevo proveedor</a>
}
```

Remember the mobile cards (`card-list`) too: they repeat the row actions inside `@if (isAdmin)`.

## Protect a route

```ts
{ path: 'reports', canActivate: [adminGuard], loadComponent: … }
```

## Hide a menu link

`core/layout/shell.ts` has `auth` as a `protected` field. The "Usuarios" link already does this in `shell.html`:

```html
@if (auth.isAdmin()) {
  <a routerLink="/reports" routerLinkActive="active" (click)="navOpen.set(false)">…</a>
}
```

## A new role (Editor)

After the backend change ([backend/auth-and-roles.md](../backend/auth-and-roles.md#b-add-a-role)):

`auth.service.ts`:

```ts
canEdit(): boolean {
  return this.role() === 'Admin' || this.role() === 'Editor';
}
```

A guard (copy `admin.guard.ts`):

```ts
export const editorGuard: CanActivateFn = () => {
  if (inject(AuthService).canEdit()) {
    return true;
  }

  return inject(Router).createUrlTree(['/products']);
};
```

Use `canEdit()` for create/edit buttons and routes, and keep `isAdmin()` for delete. Add a spec next to `admin.guard.spec.ts`.

## Verify

Log in as each demo user (`Seed__*` in `.env`):

- Buttons and menu links appear only for the right role.
- Typing a protected URL by hand redirects.
- The API returns 403 if you call it anyway (Swagger), which proves the backend enforces it.
