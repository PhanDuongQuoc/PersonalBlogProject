import type { PublicTagDetail, PublicPostSummary } from './public-post';

export interface PublicTopicSummaryResponse {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  postCount: number;
  totalViews: number;
  latestPublishedAt: string | null;
  recentPosts: PublicPostSummary[];
}

export interface PublicTopicInfoDto {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  totalPosts: number;
  totalViews: number;
}

export interface PublicTopicPostItemDto {
  id: number;
  title: string;
  slug: string;
  excerpt: string | null;
  thumbnailUrl: string | null;
  publishedAt: string | null;
  viewCount: number;
  tags: PublicTagDetail[];
}

export interface PublicTopicPostsPagedDto {
  items: PublicTopicPostItemDto[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface PublicTopicNavDto {
  id: number;
  name: string;
  slug: string;
  postCount: number;
}

export interface PublicTopicDetailResponse {
  topic: PublicTopicInfoDto;
  posts: PublicTopicPostsPagedDto;
  availableTags: PublicTagDetail[];
  otherTopics: PublicTopicNavDto[];
}
