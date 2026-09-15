export interface AdminCommentSummary {
  id: number;
  postId: number;
  postTitle: string;
  postSlug: string;
  userId: number | null;
  authorName: string;
  authorEmail: string | null;
  authorAvatar: string | null;
  content: string;
  status: "Approved" | "Pending" | "Spam" | "Rejected";
  createdAt: string;
  updatedAt: string | null;
}

export interface AdminCommentDetail {
  id: number;
  postId: number;
  postTitle: string;
  postSlug: string;
  userId: number | null;
  authorName: string;
  authorEmail: string | null;
  authorAvatar: string | null;
  content: string;
  status: "Approved" | "Pending" | "Spam" | "Rejected";
  createdAt: string;
  updatedAt: string | null;
}

export interface AdminCommentStats {
  totalComments: number;
  approvedComments: number;
  pendingComments: number;
  spamComments: number;
}

export interface UpdateCommentStatusPayload {
  status: string;
}

export interface AdminCommentListResponse {
  items: AdminCommentSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
