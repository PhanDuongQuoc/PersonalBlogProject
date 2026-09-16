export interface AdminAccountInfo {
  id: number;
  username: string;
  email: string;
  displayName: string;
  role: string;
  avatarUrl?: string | null;
  jobTitle?: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string | null;
  currentIp?: string | null;
  currentBrowser?: string | null;
}

export interface ChangePasswordPayload {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ChangePasswordResponse {
  success: boolean;
  message: string;
}
