export type ContactStatus = 'Unread' | 'Read' | 'Replied' | 'Archived';

export interface ContactMessage {
  id: number;
  name: string;
  email: string;
  subject?: string | null;
  message: string;
  status: ContactStatus;
  ipAddress?: string | null;
  replyNote?: string | null;
  repliedAt?: string | null;
  createdAt?: string | null;
  updatedAt?: string | null;
}

export interface ContactListQuery {
  pageIndex?: number | undefined;
  pageSize?: number | undefined;
  status?: string | undefined;
  keyword?: string | undefined;
}

export interface ContactListResponse {
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  unreadCount: number;
  repliedCount: number;
  items: ContactMessage[];
}

export interface UpdateContactStatusRequest {
  status: ContactStatus;
  replyNote?: string | undefined;
}

export interface ContactSummary {
  totalMessages: number;
  unreadMessages: number;
  repliedMessages: number;
  thisMonthMessages: number;
}

export interface SubmitContactPayload {
  name: string;
  email: string;
  subject?: string | undefined;
  message: string;
  botcheck?: boolean | undefined;
}

export interface SubmitContactResponse {
  success: boolean;
  message: string;
  contactId?: number | undefined;
}
