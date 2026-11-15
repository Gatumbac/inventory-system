import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { LoggerService } from '../logging/logger.service';
import { NotificationService } from '../notifications/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const logger = inject(LoggerService);
  const notifications = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      logger.error(`${req.method} ${req.url} failed`, error);

      const isLogin = req.url.endsWith('/auth/login');
      const sessionExpired = error.status === 401 && !isLogin;

      notifications.error(messageFor(error, isLogin));

      if (sessionExpired) {
        auth.logout();
        router.navigate(['/login']);
      }

      return throwError(() => error);
    }),
  );
};

function messageFor(error: HttpErrorResponse, isLogin: boolean): string {
  switch (error.status) {
    case 0:
      return 'No hay conexión con el servidor';
    case 400:
      return 'Datos inválidos';
    case 401:
      return isLogin ? 'Credenciales inválidas' : 'Sesión expirada';
    case 403:
      return 'No tienes permiso para esta acción';
    default:
      return error.error?.title ?? 'Error inesperado';
  }
}
