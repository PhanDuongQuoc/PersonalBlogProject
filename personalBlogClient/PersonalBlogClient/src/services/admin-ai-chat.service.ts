import axiosInstance from "@/boot/ApiGateway/axios";
import type { AiChatRequest, AiChatResponse } from "@/types/ai-chat";

export const adminAiChatService = {
  async sendMessage(request: AiChatRequest): Promise<AiChatResponse> {
    const res = await axiosInstance.post<AiChatResponse>("/admin/ai/chat", request);
    return res.data;
  }
};
