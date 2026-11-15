import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoggerService {
  info(message: string, ...data: unknown[]): void {
    console.info(message, ...data);
  }

  warn(message: string, ...data: unknown[]): void {
    console.warn(message, ...data);
  }

  error(message: string, ...data: unknown[]): void {
    console.error(message, ...data);
  }
}
