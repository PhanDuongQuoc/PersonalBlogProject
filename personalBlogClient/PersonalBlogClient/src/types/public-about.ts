export interface PublicAboutProfile {
  id: number;
  username: string;
  name: string;
  role: string;
  jobTitle: string;
  bio: string | null;
  aboutStory: string | null;
  location: string | null;
  phone: string | null;
  email: string;
  avatarUrl: string | null;
  cvUrl: string | null;
  githubUrl: string | null;
  linkedinUrl: string | null;
  facebookUrl: string | null;
  twitterUrl: string | null;
  websiteUrl: string | null;
  yearsOfExperience: number;
}

export interface PublicAboutSkill {
  id: number;
  name: string;
  category: string;
  proficiency: number;
  icon: string | null;
  displayOrder: number;
}

export interface PublicAboutExperience {
  id: number;
  role: string;
  company: string;
  location: string | null;
  startDate: string;
  endDate: string | null;
  isCurrent: boolean;
  description: string | null;
  technologies: string | null;
  displayOrder: number;
}

export interface PublicAboutEducation {
  id: number;
  institution: string;
  degree: string;
  startYear: string | null;
  endYear: string | null;
  description: string | null;
  displayOrder: number;
}

export interface PublicAboutStats {
  publishedPostCount: number;
  categoryCount: number;
  totalViewCount: number;
  yearsOfExperience: number;
}

export interface PublicAboutResponse {
  profile: PublicAboutProfile;
  skills: PublicAboutSkill[];
  experiences: PublicAboutExperience[];
  educations: PublicAboutEducation[];
  stats: PublicAboutStats;
}
