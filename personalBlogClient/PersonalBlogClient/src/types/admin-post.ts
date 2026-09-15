export interface AdminPostSummary {
  id: number;
  title: string;
  slug: string;
  excerpt: string | null;
  thumbnailUrl: string | null;
  status: "Published" | "Draft" | "Archived" | string;
  categoryId: number;
  categoryName: string;
  authorId: number;
  authorName: string;
  viewCount: number;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  tags: string[];
}

export interface AdminTagSummary {
  id: number;
  name: string;
  slug: string;
}

export interface AdminCategorySummary {
  id: number;
  name: string;
  slug: string;
}

export interface AdminPostDetail {
  id: number;
  title: string;
  slug: string;
  content: string;
  excerpt: string | null;
  thumbnailUrl: string | null;
  status: string;
  category: AdminCategorySummary;
  authorId: number;
  authorName: string;
  viewCount: number;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  tags: AdminTagSummary[];
}

export interface CreatePostPayload {
  title: string;
  slug?: string;
  content: string;
  excerpt?: string;
  thumbnailUrl?: string;
  status?: string;
  categoryId: number;
  tagNames?: string[];
}

export interface UpdatePostPayload {
  title: string;
  slug?: string;
  content: string;
  excerpt?: string;
  thumbnailUrl?: string;
  status?: string;
  categoryId: number;
  tagNames?: string[];
}

export interface AdminPostStats {
  totalPosts: number;
  publishedPosts: number;
  draftPosts: number;
  totalViews: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface PostFilterParams {
  search?: string;
  categoryId?: number | null;
  status?: string;
  page: number;
  pageSize: number;
}
