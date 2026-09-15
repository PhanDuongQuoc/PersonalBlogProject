import api from "@/boot/ApiGateway/axios";
import type {
  AdminTagSummary,
  AdminTagDetail,
  AdminTagStats,
  CreateTagPayload,
  UpdateTagPayload,
  AdminTagListResponse
} from "@/types/admin-tag";

export const adminTagService = {
  /**
   * Lấy danh sách thẻ phân trang & tìm kiếm
   */
  async getTags(params: { search?: string; page: number; pageSize: number }): Promise<AdminTagListResponse> {
    const query: Record<string, string | number> = {
      page: params.page,
      pageSize: params.pageSize
    };

    if (params.search && params.search.trim()) {
      query.search = params.search.trim();
    }

    const res = await api.get<AdminTagListResponse>("/admin/tags", { params: query });
    return res.data;
  },

  /**
   * Lấy số liệu thống kê thẻ
   */
  async getStats(): Promise<AdminTagStats> {
    const res = await api.get<AdminTagStats>("/admin/tags/stats");
    return res.data;
  },

  /**
   * Lấy chi tiết thẻ theo ID
   */
  async getTagById(id: number): Promise<AdminTagDetail> {
    const res = await api.get<AdminTagDetail>(`/admin/tags/${id}`);
    return res.data;
  },

  /**
   * Tạo thẻ mới
   */
  async createTag(payload: CreateTagPayload): Promise<AdminTagDetail> {
    const res = await api.post<AdminTagDetail>("/admin/tags", payload);
    return res.data;
  },

  /**
   * Cập nhật thẻ
   */
  async updateTag(id: number, payload: UpdateTagPayload): Promise<AdminTagDetail> {
    const res = await api.put<AdminTagDetail>(`/admin/tags/${id}`, payload);
    return res.data;
  },

  /**
   * Xóa thẻ
   */
  async deleteTag(id: number): Promise<void> {
    await api.delete(`/admin/tags/${id}`);
  }
};
