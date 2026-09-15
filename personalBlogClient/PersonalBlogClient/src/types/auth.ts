export interface AuthUser {
  id: number;
  username: string;
  email: string;
  displayName: string;
  role: string;
  avatarUrl?: string | null;
  jobTitle?: string | null;
}

export interface LoginPayload {
  usernameOrEmail: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: AuthUser;
}

export interface ForgotPasswordResponse {
  message: string;
  otp?: string;
  email: string;
}

export interface ResetPasswordPayload {
  email: string;
  otp: string;
  newPassword: string;
}
