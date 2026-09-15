using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Admin;

// ==========================================
// 1. Profile DTOs
// ==========================================

public class AdminProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = null!;
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? AboutStory { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? CvUrl { get; set; }
    public string? GithubUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public int? YearsOfExperience { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateAdminProfileRequest
{
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? JobTitle { get; set; }
    public string? Bio { get; set; }
    public string? AboutStory { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? CvUrl { get; set; }
    public string? GithubUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public int? YearsOfExperience { get; set; }
}

// ==========================================
// 2. Skill DTOs
// ==========================================

public class AdminSkillDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Proficiency { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSkillRequest
{
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Proficiency { get; set; } = 80;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateSkillRequest
{
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public int Proficiency { get; set; } = 80;
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

// ==========================================
// 3. Experience DTOs
// ==========================================

public class AdminExperienceDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string? Location { get; set; }
    public string StartDate { get; set; } = null!;
    public string? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Technologies { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExperienceRequest
{
    public string Role { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string? Location { get; set; }
    public string StartDate { get; set; } = null!;
    public string? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Technologies { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateExperienceRequest
{
    public string Role { get; set; } = null!;
    public string Company { get; set; } = null!;
    public string? Location { get; set; }
    public string StartDate { get; set; } = null!;
    public string? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Technologies { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

// ==========================================
// 4. Education DTOs
// ==========================================

public class AdminEducationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Institution { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEducationRequest
{
    public string Institution { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateEducationRequest
{
    public string Institution { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string? StartYear { get; set; }
    public string? EndYear { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

// ==========================================
// 5. Stats Overview DTO
// ==========================================

public class AdminSkillsOverviewStatsDto
{
    public int TotalSkills { get; set; }
    public int TotalExperiences { get; set; }
    public int TotalEducations { get; set; }
    public int HighProficiencySkillsCount { get; set; }
}
