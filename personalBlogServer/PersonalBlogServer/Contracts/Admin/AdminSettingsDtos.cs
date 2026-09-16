using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Admin;

public class SiteGeneralSettings
{
    public string SiteTitle { get; set; } = "PDQ Portfolio & Tech Blog";
    public string Tagline { get; set; } = "Chia sẻ kiến trúc phần mềm, Vue.js, .NET & Cloud Computing";
    public string SiteDescription { get; set; } = "Không gian chia sẻ kiến thức chuyên sâu về công nghệ, phát triển phần mềm và hành trình sáng tạo kỹ thuật của Phan Duong Quoc.";
    public string FooterCopyright { get; set; } = "© 2026 Phan Duong Quoc. All rights reserved.";
    public string LogoText { get; set; } = "P.";
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
}

public class SeoMetaSettings
{
    public string MetaTitleTemplate { get; set; } = "%title% · PDQ Blog";
    public string DefaultKeywords { get; set; } = "Vue.js, .NET 9, Clean Architecture, PostgreSQL, Quasar, Tailwind, Frontend, Backend, Software Engineer";
    public string? DefaultOgImageUrl { get; set; }
    public string? GoogleAnalyticsId { get; set; }
    public string? CanonicalBaseUrl { get; set; } = "https://phanduongquoc.dev";
}

public class ReadingSettings
{
    public int PostsPerPage { get; set; } = 9;
    public bool ShowReadingTime { get; set; } = true;
    public bool ShowPublicViewCount { get; set; } = true;
    public bool ShowRelatedPosts { get; set; } = true;
    public bool ShowAuthorBioInPosts { get; set; } = true;
}

public class CommentSettings
{
    public bool EnableComments { get; set; } = true;
    public bool RequireModeration { get; set; } = true;
    public bool AllowGuestComments { get; set; } = true;
    public string ForbiddenKeywords { get; set; } = "spam, scam, quảng cáo, cờ bạc, lừa đảo, casino";
}

public class ContactSettings
{
    public string ContactEmail { get; set; } = "phanduongquoc111@gmail.com";
    public string? Phone { get; set; } = "+84 987 654 321";
    public string? Location { get; set; } = "Hồ Chí Minh, Việt Nam";
    public string? GithubUrl { get; set; } = "https://github.com/phanduongquoc11";
    public string? LinkedinUrl { get; set; } = "https://linkedin.com/in/phanduongquoc";
    public string? FacebookUrl { get; set; } = "https://facebook.com/phanduongquoc";
    public string? TwitterUrl { get; set; } = "https://twitter.com";
    public string? YoutubeUrl { get; set; }
    public string? WebsiteUrl { get; set; } = "https://phanduongquoc.dev";
}

public class SystemMaintenanceSettings
{
    public bool MaintenanceMode { get; set; } = false;
    public string MaintenanceNotice { get; set; } = "Hệ thống đang được bảo trì nâng cấp định kỳ. Vui lòng quay lại sau ít phút!";
    public DateTime? LastCacheClearedAt { get; set; }
    public DateTime? LastSettingsUpdatedAt { get; set; }
}

public class SiteSettingsDto
{
    public SiteGeneralSettings General { get; set; } = new();
    public SeoMetaSettings Seo { get; set; } = new();
    public ReadingSettings Reading { get; set; } = new();
    public CommentSettings Comments { get; set; } = new();
    public ContactSettings Contact { get; set; } = new();
    public SystemMaintenanceSettings Maintenance { get; set; } = new();
}
