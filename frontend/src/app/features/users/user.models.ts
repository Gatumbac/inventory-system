export type UserRole = 'Admin' | 'Viewer';

export interface User {
  id: number;
  name: string;
  lastName: string;
  email: string;
  role: UserRole;
}

export interface UserRequest {
  name: string;
  lastName: string;
  email: string;
  password: string;
  role: UserRole;
}
