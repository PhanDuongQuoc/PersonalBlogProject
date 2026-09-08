export interface PublicCategoryDetail {
  id: number;
  name: string;
  slug: string;
}

export interface PublicAuthorDetail {
  id: number;
  username: string;
  name: string;
  role: string;
  avatarUrl: string | null;
  bio: string | null;
  jobTitle: string | null;
}

export interface PublicTagDetail {
  id: number;
  name: string;
  slug: string;
}

export interface PublicCommentDetail {
  id: number;
  guestName: string | null;
  content: string;
  createdAt: string;
}

export interface PublicPostSummary {
  id: number;
  title: string;
  slug: string;
  excerpt: string | null;
  thumbnailUrl: string | null;
  category: string;
  publishedAt: string | null;
  viewCount: number;
}

export interface PublicPostDetailResponse {
  id: number;
  title: string;
  slug: string;
  content: string;
  excerpt: string | null;
  thumbnailUrl: string | null;
  publishedAt: string | null;
  viewCount: number;
  category: PublicCategoryDetail;
  author: PublicAuthorDetail;
  tags: PublicTagDetail[];
  comments: PublicCommentDetail[];
  relatedPosts: PublicPostSummary[];
}
