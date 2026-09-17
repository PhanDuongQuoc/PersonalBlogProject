export interface ChatMessage {
  id?: string;
  role: 'user' | 'model' | 'assistant';
  content: string;
  timestamp?: Date | string;
  isError?: boolean;
}

export interface AiChatRequest {
  message: string;
  history?: Array<{
    role: string;
    content: string;
  }>;
}

export interface AiChatResponse {
  success: boolean;
  reply: string;
  error?: string | null;
  suggestedFollowUps?: string[];
}
