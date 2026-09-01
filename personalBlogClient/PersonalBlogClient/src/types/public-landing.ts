export interface PublicProfile {
  id: number;
  username: string;
  name: string;
  role: string;
  avatarUrl: string | null;
  email: string;
}

export interface PublicPost {
  id: number;
  title: string;
  slug: string;
  excerpt: string | null;
  thumbnailUrl: string | null;
  category: string;
  publishedAt: string | null;
  viewCount: number;
}

export interface PublicLandingResponse {
  profile: PublicProfile;
  featuredPosts: PublicPost[];
  categories: {
    name: string;
    slug: string;
    postCount: number;
  }[];
  stats: {
    publishedPostCount: number;
    categoryCount: number;
    totalViewCount: number;
  };
}
