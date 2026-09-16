import axiosInstance from "@/boot/ApiGateway/axios";
import type { SiteSettings, UpdateSettingsResponse } from "@/types/admin-settings";

export const adminSettingsService = {
  async getSettings(): Promise<SiteSettings> {
    const res = await axiosInstance.get<SiteSettings>("/admin/settings");
    return res.data;
  },

  async updateSettings(settings: SiteSettings): Promise<UpdateSettingsResponse> {
    const res = await axiosInstance.put<UpdateSettingsResponse>("/admin/settings", settings);
    return res.data;
  },

  async clearCache(): Promise<{ success: boolean; message: string }> {
    const res = await axiosInstance.post<{ success: boolean; message: string }>("/admin/settings/clear-cache");
    return res.data;
  }
};
