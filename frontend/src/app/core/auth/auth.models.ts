export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  role: string;
}

export interface CurrentUser {
  name: string;
  lastName: string;
  email: string;
  role: string;
}
