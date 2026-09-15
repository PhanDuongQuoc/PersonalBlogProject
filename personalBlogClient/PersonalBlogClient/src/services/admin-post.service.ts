import api from "@/boot/ApiGateway/axios";
import type {
  AdminPostSummary,
  AdminPostDetail,
  AdminPostStats,
  AdminCategorySummary,
  AdminTagSummary,
  CreatePostPayload,
  UpdatePostPayload,
  PagedResult,
  PostFilterParams
} from "@/types/admin-post";

export const adminPostService = {
  /**
   * Lấy danh sách bài viết phân trang & bộ lọc
   */
  async getPosts(params: PostFilterParams): Promise<PagedResult<AdminPostSummary>> {
    const query: Record<string, string | number> = {
      page: params.page,
      pageSize: params.pageSize
    };

    if (params.search && params.search.trim()) {
      query.search = params.search.trim();
    }
    if (params.categoryId && params.categoryId > 0) {
      query.categoryId = params.categoryId;
    }
    if (params.status && params.status !== "all") {
      query.status = params.status;
    }

    const res = await api.get<PagedResult<AdminPostSummary>>("/admin/posts", { params: query });
    return res.data;
  },

  /**
   * Lấy số liệu thống kê bài viết
   */
  async getStats(): Promise<AdminPostStats> {
    const res = await api.get<AdminPostStats>("/admin/posts/stats");
    return res.data;
  },

  /**
   * Lấy danh mục và tags cho dropdowns
   */
  async getMetadata(): Promise<{ categories: AdminCategorySummary[]; tags: AdminTagSummary[] }> {
    const res = await api.get<{ categories: AdminCategorySummary[]; tags: AdminTagSummary[] }>("/admin/posts/metadata");
    return res.data;
  },

  /**
   * Lấy chi tiết bài viết theo ID
   */
  async getPostById(id: number): Promise<AdminPostDetail> {
    const res = await api.get<AdminPostDetail>(`/admin/posts/${id}`);
    return res.data;
  },

  /**
   * Tạo bài viết mới
   */
  async createPost(payload: CreatePostPayload): Promise<AdminPostDetail> {
    const res = await api.post<AdminPostDetail>("/admin/posts", payload);
    return res.data;
  },

  /**
   * Cập nhật bài viết
   */
  async updatePost(id: number, payload: UpdatePostPayload): Promise<AdminPostDetail> {
    const res = await api.put<AdminPostDetail>(`/admin/posts/${id}`, payload);
    return res.data;
  },

  /**
   * Cập nhật nhanh trạng thái bài viết (Published / Draft)
   */
  async updateStatus(id: number, status: string): Promise<void> {
    await api.patch(`/admin/posts/${id}/status`, { status });
  },

  /**
   * Xóa bài viết
   */
  async deletePost(id: number): Promise<void> {
    await api.delete(`/admin/posts/${id}`);
  }
};
