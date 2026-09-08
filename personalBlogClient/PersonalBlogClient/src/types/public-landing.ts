export interface PublicProfile {
  id: number;
  username: string;
  name: string;
  role: string;
  jobTitle: string | null;
  bio: string | null;
  location: string | null;
  phone: string | null;
  avatarUrl: string | null;
  email: string;
  cvUrl: string | null;
  githubUrl: string | null;
  linkedinUrl: string | null;
  facebookUrl: string | null;
  twitterUrl: string | null;
  websiteUrl: string | null;
  yearsOfExperience: number;
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
    yearsOfExperience: number;
  };
}
