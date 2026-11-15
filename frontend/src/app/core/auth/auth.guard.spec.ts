import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { provideRouter } from '@angular/router';
import { authGuard } from './auth.guard';

describe('authGuard', () => {
  const run = () =>
    TestBed.runInInjectionContext(() =>
      authGuard({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot),
    );

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('redirects to /login when there is no token', () => {
    const result = run();
    expect(result).toEqual(TestBed.inject(Router).createUrlTree(['/login']));
  });

  it('allows navigation when there is a token', () => {
    localStorage.setItem('token', 'abc');
    expect(run()).toBe(true);
  });
});
