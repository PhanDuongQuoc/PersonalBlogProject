import api from "@/boot/ApiGateway/axios";
import type {
  AdminCategorySummary,
  AdminCategoryDetail,
  AdminCategoryStats,
  CreateCategoryPayload,
  UpdateCategoryPayload,
  AdminCategoryListResponse
} from "@/types/admin-category";

export const adminCategoryService = {
  /**
   * Lấy danh sách danh mục phân trang & tìm kiếm
   */
  async getCategories(params: { search?: string; page: number; pageSize: number }): Promise<AdminCategoryListResponse> {
    const query: Record<string, string | number> = {
      page: params.page,
      pageSize: params.pageSize
    };

    if (params.search && params.search.trim()) {
      query.search = params.search.trim();
    }

    const res = await api.get<AdminCategoryListResponse>("/admin/categories", { params: query });
    return res.data;
  },

  /**
   * Lấy số liệu thống kê danh mục
   */
  async getStats(): Promise<AdminCategoryStats> {
    const res = await api.get<AdminCategoryStats>("/admin/categories/stats");
    return res.data;
  },

  /**
   * Lấy chi tiết danh mục theo ID
   */
  async getCategoryById(id: number): Promise<AdminCategoryDetail> {
    const res = await api.get<AdminCategoryDetail>(`/admin/categories/${id}`);
    return res.data;
  },

  /**
   * Tạo danh mục mới
   */
  async createCategory(payload: CreateCategoryPayload): Promise<AdminCategoryDetail> {
    const res = await api.post<AdminCategoryDetail>("/admin/categories", payload);
    return res.data;
  },

  /**
   * Cập nhật danh mục
   */
  async updateCategory(id: number, payload: UpdateCategoryPayload): Promise<AdminCategoryDetail> {
    const res = await api.put<AdminCategoryDetail>(`/admin/categories/${id}`, payload);
    return res.data;
  },

  /**
   * Xóa danh mục
   */
  async deleteCategory(id: number): Promise<void> {
    await api.delete(`/admin/categories/${id}`);
  }
};
