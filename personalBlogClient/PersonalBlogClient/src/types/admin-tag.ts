export interface AdminTagSummary {
  id: number;
  name: string;
  slug: string;
  postCount: number;
  createdAt: string;
}

export interface AdminTagDetail {
  id: number;
  name: string;
  slug: string;
  postCount: number;
  createdAt: string;
}

export interface AdminTagStats {
  totalTags: number;
  totalAssignedPosts: number;
  tagsWithNoPosts: number;
  topTagName: string | null;
  topTagPostCount: number;
}

export interface CreateTagPayload {
  name: string;
  slug?: string | null | undefined;
}

export interface UpdateTagPayload {
  name: string;
  slug?: string | null | undefined;
}

export interface AdminTagListResponse {
  items: AdminTagSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
