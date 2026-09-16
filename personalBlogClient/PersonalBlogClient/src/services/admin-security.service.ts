import axiosInstance from "@/boot/ApiGateway/axios";
import type {
  AdminAccountInfo,
  ChangePasswordPayload,
  ChangePasswordResponse
} from "@/types/admin-security";

export const adminSecurityService = {
  async getAccountInfo(): Promise<AdminAccountInfo> {
    const res = await axiosInstance.get<AdminAccountInfo>("/admin/security/account-info");
    return res.data;
  },

  async changePassword(payload: ChangePasswordPayload): Promise<ChangePasswordResponse> {
    const res = await axiosInstance.post<ChangePasswordResponse>("/admin/security/change-password", payload);
    return res.data;
  }
};
