import { TestBed } from '@angular/core/testing';
import {
  ActivatedRouteSnapshot,
  Router,
  RouterStateSnapshot,
  provideRouter,
} from '@angular/router';
import { adminGuard } from './admin.guard';

describe('adminGuard', () => {
  const run = () =>
    TestBed.runInInjectionContext(() =>
      adminGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot),
    );

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('allows navigation for an admin', () => {
    localStorage.setItem('role', 'Admin');
    expect(run()).toBe(true);
  });

  it('redirects to /products for a viewer', () => {
    localStorage.setItem('role', 'Viewer');
    expect(run()).toEqual(TestBed.inject(Router).createUrlTree(['/products']));
  });
});
