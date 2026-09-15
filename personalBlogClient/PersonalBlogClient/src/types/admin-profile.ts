export interface AdminProfile {
  id: number;
  username: string;
  email: string;
  displayName: string | null;
  avatarUrl: string | null;
  role: string;
  jobTitle: string | null;
  bio: string | null;
  aboutStory: string | null;
  location: string | null;
  phone: string | null;
  cvUrl: string | null;
  githubUrl: string | null;
  linkedinUrl: string | null;
  facebookUrl: string | null;
  twitterUrl: string | null;
  websiteUrl: string | null;
  yearsOfExperience: number | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface UpdateAdminProfilePayload {
  displayName?: string | null | undefined;
  avatarUrl?: string | null | undefined;
  jobTitle?: string | null | undefined;
  bio?: string | null | undefined;
  aboutStory?: string | null | undefined;
  location?: string | null | undefined;
  phone?: string | null | undefined;
  cvUrl?: string | null | undefined;
  githubUrl?: string | null | undefined;
  linkedinUrl?: string | null | undefined;
  facebookUrl?: string | null | undefined;
  twitterUrl?: string | null | undefined;
  websiteUrl?: string | null | undefined;
  yearsOfExperience?: number | null | undefined;
}

export interface AdminSkill {
  id: number;
  userId: number;
  name: string;
  category: string;
  proficiency: number;
  icon: string | null;
  displayOrder: number;
  createdAt: string;
}

export interface CreateSkillPayload {
  name: string;
  category: string;
  proficiency: number;
  icon?: string | null | undefined;
  displayOrder: number;
}

export interface UpdateSkillPayload {
  name: string;
  category: string;
  proficiency: number;
  icon?: string | null | undefined;
  displayOrder: number;
}

export interface AdminExperience {
  id: number;
  userId: number;
  role: string;
  company: string;
  location: string | null;
  startDate: string;
  endDate: string | null;
  isCurrent: boolean;
  description: string | null;
  technologies: string | null;
  displayOrder: number;
  createdAt: string;
}

export interface CreateExperiencePayload {
  role: string;
  company: string;
  location?: string | null | undefined;
  startDate: string;
  endDate?: string | null | undefined;
  isCurrent: boolean;
  description?: string | null | undefined;
  technologies?: string | null | undefined;
  displayOrder: number;
}

export interface UpdateExperiencePayload {
  role: string;
  company: string;
  location?: string | null | undefined;
  startDate: string;
  endDate?: string | null | undefined;
  isCurrent: boolean;
  description?: string | null | undefined;
  technologies?: string | null | undefined;
  displayOrder: number;
}

export interface AdminEducation {
  id: number;
  userId: number;
  institution: string;
  degree: string;
  startYear: string | null;
  endYear: string | null;
  description: string | null;
  displayOrder: number;
  createdAt: string;
}

export interface CreateEducationPayload {
  institution: string;
  degree: string;
  startYear?: string | null | undefined;
  endYear?: string | null | undefined;
  description?: string | null | undefined;
  displayOrder: number;
}

export interface UpdateEducationPayload {
  institution: string;
  degree: string;
  startYear?: string | null | undefined;
  endYear?: string | null | undefined;
  description?: string | null | undefined;
  displayOrder: number;
}

export interface AdminSkillsOverviewStats {
  totalSkills: number;
  totalExperiences: number;
  totalEducations: number;
  highProficiencySkillsCount: number;
}
