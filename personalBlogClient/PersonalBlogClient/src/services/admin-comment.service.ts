import api from "@/boot/ApiGateway/axios";
import type {
  AdminCommentSummary,
  AdminCommentDetail,
  AdminCommentStats,
  AdminCommentListResponse
} from "@/types/admin-comment";

export const adminCommentService = {
  /**
   * Lấy danh sách bình luận phân trang, tìm kiếm & lọc trạng thái
   */
  async getComments(params: {
    search?: string;
    status?: string;
    postId?: number | null;
    page: number;
    pageSize: number;
  }): Promise<AdminCommentListResponse> {
    const query: Record<string, string | number> = {
      page: params.page,
      pageSize: params.pageSize
    };

    if (params.search && params.search.trim()) {
      query.search = params.search.trim();
    }

    if (params.status && params.status !== "all") {
      query.status = params.status;
    }

    if (params.postId && params.postId > 0) {
      query.postId = params.postId;
    }

    const res = await api.get<AdminCommentListResponse>("/admin/comments", { params: query });
    return res.data;
  },

  /**
   * Lấy số liệu thống kê bình luận
   */
  async getStats(): Promise<AdminCommentStats> {
    const res = await api.get<AdminCommentStats>("/admin/comments/stats");
    return res.data;
  },

  /**
   * Lấy chi tiết bình luận theo ID
   */
  async getCommentById(id: number): Promise<AdminCommentDetail> {
    const res = await api.get<AdminCommentDetail>(`/admin/comments/${id}`);
    return res.data;
  },

  /**
   * Cập nhật trạng thái bình luận (Approved, Pending, Spam, Rejected)
   */
  async updateStatus(id: number, status: string): Promise<void> {
    await api.patch(`/admin/comments/${id}/status`, { status });
  },

  /**
   * Xóa bình luận
   */
  async deleteComment(id: number): Promise<void> {
    await api.delete(`/admin/comments/${id}`);
  }
};
