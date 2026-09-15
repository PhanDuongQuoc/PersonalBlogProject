export interface AdminAnalyticsSummary {
  totalViews: number;
  viewsGrowthRate: number;
  totalPosts: number;
  publishedPosts: number;
  draftPosts: number;
  archivedPosts: number;
  totalComments: number;
  approvedComments: number;
  pendingComments: number;
  totalCategories: number;
  totalTags: number;
}

export interface ViewsTrendPoint {
  dateLabel: string;
  views: number;
  uniqueReaders: number;
}

export interface ViewsTrendData {
  period: string;
  totalViewsInPeriod: number;
  dataPoints: ViewsTrendPoint[];
}

export interface CategoryAnalyticsItem {
  categoryId: number;
  categoryName: string;
  slug: string;
  postCount: number;
  totalViews: number;
  percentage: number;
}

export interface MonthlyCommentsItem {
  month: number;
  monthLabel: string;
  totalComments: number;
  approvedComments: number;
}

export interface TopPerformingPost {
  id: number;
  title: string;
  slug: string;
  categoryName: string;
  viewCount: number;
  commentCount: number;
  publishedAt?: string | null;
  thumbnailUrl?: string | null;
}
