import { defineStore } from "pinia";
import { computed, ref } from "vue";
import api from "@/boot/ApiGateway/axios";
import type {
  AuthUser,
  ForgotPasswordResponse,
  LoginPayload,
  LoginResponse,
  ResetPasswordPayload
} from "@/types/auth";

const TOKEN_KEY = "pdq_auth_token";
const USER_KEY = "pdq_auth_user";

export const useAuthStore = defineStore("auth", () => {
  const token = ref<string | null>(localStorage.getItem(TOKEN_KEY));
  const user = ref<AuthUser | null>(
    (() => {
      try {
        const raw = localStorage.getItem(USER_KEY);
        return raw ? JSON.parse(raw) : null;
      } catch {
        return null;
      }
    })()
  );
  const isLoading = ref<boolean>(false);
  const errorMessage = ref<string | null>(null);

  const isAuthenticated = computed(() => !!token.value);
  const userDisplayName = computed(() => user.value?.displayName || user.value?.username || "Admin");
  const userRole = computed(() => user.value?.role || "Author");
  const userAvatar = computed(() => user.value?.avatarUrl || "");

  async function login(payload: LoginPayload): Promise<{ success: boolean; message?: string }> {
    isLoading.value = true;
    errorMessage.value = null;

    try {
      const response = await api.post<LoginResponse>("/auth/login", {
        usernameOrEmail: payload.usernameOrEmail,
        password: payload.password
      });

      const data = response.data;
      token.value = data.token;
      user.value = data.user;

      localStorage.setItem(TOKEN_KEY, data.token);
      localStorage.setItem(USER_KEY, JSON.stringify(data.user));

      return { success: true };
    } catch (error: any) {
      const msg =
        error.response?.data?.message ||
        (error.response?.data?.errors
          ? Object.values(error.response.data.errors).flat().join(" ")
          : null) ||
        "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin!";
      errorMessage.value = msg;
      return { success: false, message: msg };
    } finally {
      isLoading.value = false;
    }
  }

  async function forgotPassword(email: string): Promise<{ success: boolean; message?: string; otp?: string }> {
    isLoading.value = true;
    errorMessage.value = null;

    try {
      const response = await api.post<ForgotPasswordResponse>("/auth/forgot-password", { email });
      return {
        success: true,
        message: response.data.message,
        ...(response.data.otp ? { otp: response.data.otp } : {})
      };
    } catch (error: any) {
      const msg =
        error.response?.data?.message ||
        (error.response?.data?.errors
          ? Object.values(error.response.data.errors).flat().join(" ")
          : null) ||
        "Gửi yêu cầu thất bại. Vui lòng kiểm tra lại email!";
      errorMessage.value = msg;
      return { success: false, message: msg };
    } finally {
      isLoading.value = false;
    }
  }

  async function resetPassword(payload: ResetPasswordPayload): Promise<{ success: boolean; message?: string }> {
    isLoading.value = true;
    errorMessage.value = null;

    try {
      const response = await api.post<{ message: string }>("/auth/reset-password", payload);
      return {
        success: true,
        message: response.data.message
      };
    } catch (error: any) {
      const msg =
        error.response?.data?.message ||
        (error.response?.data?.errors
          ? Object.values(error.response.data.errors).flat().join(" ")
          : null) ||
        "Đặt lại mật khẩu thất bại. Vui lòng thử lại!";
      errorMessage.value = msg;
      return { success: false, message: msg };
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchCurrentUser(): Promise<AuthUser | null> {
    if (!token.value) return null;

    try {
      const response = await api.get<AuthUser>("/auth/me");
      user.value = response.data;
      localStorage.setItem(USER_KEY, JSON.stringify(response.data));
      return response.data;
    } catch {
      logout();
      return null;
    }
  }

  function logout() {
    token.value = null;
    user.value = null;
    errorMessage.value = null;
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  return {
    token,
    user,
    isLoading,
    errorMessage,
    isAuthenticated,
    userDisplayName,
    userRole,
    userAvatar,
    login,
    forgotPassword,
    resetPassword,
    fetchCurrentUser,
    logout
  };
});
