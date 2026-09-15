export interface AdminCategorySummary {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  thumbnailUrl: string | null;
  postCount: number;
  createdAt: string;
}

export interface AdminCategoryDetail {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  thumbnailUrl: string | null;
  postCount: number;
  createdAt: string;
}

export interface AdminCategoryStats {
  totalCategories: number;
  totalAssignedPosts: number;
  categoriesWithNoPosts: number;
  topCategoryName: string | null;
  topCategoryPostCount: number;
}

export interface CreateCategoryPayload {
  name: string;
  slug?: string | null | undefined;
  description?: string | null | undefined;
  thumbnailUrl?: string | null | undefined;
}

export interface UpdateCategoryPayload {
  name: string;
  slug?: string | null | undefined;
  description?: string | null | undefined;
  thumbnailUrl?: string | null | undefined;
}

export interface AdminCategoryListResponse {
  items: AdminCategorySummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
