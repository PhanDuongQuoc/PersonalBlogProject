export interface SiteGeneralSettings {
  siteTitle: string;
  tagline: string;
  siteDescription: string;
  footerCopyright: string;
  logoText: string;
  logoUrl?: string | null;
  faviconUrl?: string | null;
}

export interface SeoMetaSettings {
  metaTitleTemplate: string;
  defaultKeywords: string;
  defaultOgImageUrl?: string | null;
  googleAnalyticsId?: string | null;
  canonicalBaseUrl?: string | null;
}

export interface ReadingSettings {
  postsPerPage: number;
  showReadingTime: boolean;
  showPublicViewCount: boolean;
  showRelatedPosts: boolean;
  showAuthorBioInPosts: boolean;
}

export interface CommentSettings {
  enableComments: boolean;
  requireModeration: boolean;
  allowGuestComments: boolean;
  forbiddenKeywords: string;
}

export interface ContactSettings {
  contactEmail: string;
  phone?: string | null;
  location?: string | null;
  githubUrl?: string | null;
  linkedinUrl?: string | null;
  facebookUrl?: string | null;
  twitterUrl?: string | null;
  youtubeUrl?: string | null;
  websiteUrl?: string | null;
}

export interface SystemMaintenanceSettings {
  maintenanceMode: boolean;
  maintenanceNotice: string;
  lastCacheClearedAt?: string | null;
  lastSettingsUpdatedAt?: string | null;
}

export interface SiteSettings {
  general: SiteGeneralSettings;
  seo: SeoMetaSettings;
  reading: ReadingSettings;
  comments: CommentSettings;
  contact: ContactSettings;
  maintenance: SystemMaintenanceSettings;
}

export interface UpdateSettingsResponse {
  success: boolean;
  message: string;
  data: SiteSettings;
}
