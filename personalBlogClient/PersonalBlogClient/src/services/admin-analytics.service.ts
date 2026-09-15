import axiosInstance from "@/boot/ApiGateway/axios";
import type {
  AdminAnalyticsSummary,
  ViewsTrendData,
  CategoryAnalyticsItem,
  MonthlyCommentsItem,
  TopPerformingPost
} from "@/types/admin-analytics";

export const adminAnalyticsService = {
  async getSummary(): Promise<AdminAnalyticsSummary> {
    const res = await axiosInstance.get<AdminAnalyticsSummary>("/admin/analytics/summary");
    return res.data;
  },

  async getViewsTrend(period: "7d" | "30d" | "monthly" | "yearly" = "monthly"): Promise<ViewsTrendData> {
    const res = await axiosInstance.get<ViewsTrendData>("/admin/analytics/views-trend", {
      params: { period }
    });
    return res.data;
  },

  async getCategoryDistribution(): Promise<CategoryAnalyticsItem[]> {
    const res = await axiosInstance.get<CategoryAnalyticsItem[]>("/admin/analytics/category-distribution");
    return res.data;
  },

  async getCommentsTrend(year?: number): Promise<MonthlyCommentsItem[]> {
    const res = await axiosInstance.get<MonthlyCommentsItem[]>("/admin/analytics/comments-trend", {
      params: { year }
    });
    return res.data;
  },

  async getTopPosts(limit: number = 5): Promise<TopPerformingPost[]> {
    const res = await axiosInstance.get<TopPerformingPost[]>("/admin/analytics/top-posts", {
      params: { limit }
    });
    return res.data;
  }
};
