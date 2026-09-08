using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Public;

public sealed record PublicAboutResponse(
    PublicAboutProfileResponse Profile,
    IReadOnlyList<PublicAboutSkillResponse> Skills,
    IReadOnlyList<PublicAboutExperienceResponse> Experiences,
    IReadOnlyList<PublicAboutEducationResponse> Educations,
    PublicAboutStatsResponse Stats);

public sealed record PublicAboutProfileResponse(
    int Id,
    string Username,
    string Name,
    string Role,
    string? JobTitle,
    string? Bio,
    string? AboutStory,
    string? Location,
    string? Phone,
    string Email,
    string? AvatarUrl,
    string? CvUrl,
    string? GithubUrl,
    string? LinkedinUrl,
    string? FacebookUrl,
    string? TwitterUrl,
    string? WebsiteUrl,
    int YearsOfExperience);

public sealed record PublicAboutSkillResponse(
    int Id,
    string Name,
    string Category,
    int Proficiency,
    string? Icon,
    int DisplayOrder);

public sealed record PublicAboutExperienceResponse(
    int Id,
    string Role,
    string Company,
    string? Location,
    string StartDate,
    string? EndDate,
    bool IsCurrent,
    string? Description,
    string? Technologies,
    int DisplayOrder);

public sealed record PublicAboutEducationResponse(
    int Id,
    string Institution,
    string Degree,
    string? StartYear,
    string? EndYear,
    string? Description,
    int DisplayOrder);

public sealed record PublicAboutStatsResponse(
    int PublishedPostCount,
    int CategoryCount,
    int TotalViewCount,
    int YearsOfExperience);
