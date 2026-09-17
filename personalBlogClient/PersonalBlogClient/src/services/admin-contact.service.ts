import axiosInstance from "@/boot/ApiGateway/axios";
import type {
  ContactListQuery,
  ContactListResponse,
  ContactMessage,
  ContactSummary,
  UpdateContactStatusRequest
} from "@/types/admin-contact";

export const adminContactService = {
  async getContacts(query?: ContactListQuery): Promise<ContactListResponse> {
    const res = await axiosInstance.get<ContactListResponse>("/admin/contacts", {
      params: query
    });
    return res.data;
  },

  async getSummary(): Promise<ContactSummary> {
    const res = await axiosInstance.get<ContactSummary>("/admin/contacts/summary");
    return res.data;
  },

  async getContactById(id: number, autoMarkAsRead = true): Promise<ContactMessage> {
    const res = await axiosInstance.get<ContactMessage>(`/admin/contacts/${id}`, {
      params: { autoMarkAsRead }
    });
    return res.data;
  },

  async updateStatus(id: number, request: UpdateContactStatusRequest): Promise<{ success: boolean; message: string; data: ContactMessage }> {
    const res = await axiosInstance.patch<{ success: boolean; message: string; data: ContactMessage }>(`/admin/contacts/${id}/status`, request);
    return res.data;
  },

  async deleteContact(id: number): Promise<{ success: boolean; message: string }> {
    const res = await axiosInstance.delete<{ success: boolean; message: string }>(`/admin/contacts/${id}`);
    return res.data;
  }
};
