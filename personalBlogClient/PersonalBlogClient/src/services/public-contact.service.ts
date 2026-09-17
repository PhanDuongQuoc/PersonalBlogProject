import axiosInstance from "@/boot/ApiGateway/axios";
import type { SubmitContactPayload, SubmitContactResponse } from "@/types/admin-contact";

export const publicContactService = {
  async submitContact(payload: SubmitContactPayload): Promise<SubmitContactResponse> {
    const res = await axiosInstance.post<SubmitContactResponse>("/public/contact", payload);
    return res.data;
  }
};
