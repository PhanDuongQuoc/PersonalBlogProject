using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalBlogServer.Contracts.Admin;
using PersonalBlogServer.Models;

namespace PersonalBlogServer.Services.Admin;

public class AdminProfileService : IAdminProfileService
{
    private readonly AppDbContext _context;

    public AdminProfileService(AppDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 1. Profile Management
    // ==========================================

    public async Task<AdminProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return MapToProfileDto(user);
    }

    public async Task<AdminProfileDto?> UpdateProfileAsync(int userId, UpdateAdminProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.DisplayName = request.DisplayName?.Trim();
        user.AvatarUrl = request.AvatarUrl?.Trim();
        user.JobTitle = request.JobTitle?.Trim();
        user.Bio = request.Bio?.Trim();
        user.AboutStory = request.AboutStory?.Trim();
        user.Location = request.Location?.Trim();
        user.Phone = request.Phone?.Trim();
        user.CvUrl = request.CvUrl?.Trim();
        user.GithubUrl = request.GithubUrl?.Trim();
        user.LinkedinUrl = request.LinkedinUrl?.Trim();
        user.FacebookUrl = request.FacebookUrl?.Trim();
        user.TwitterUrl = request.TwitterUrl?.Trim();
        user.WebsiteUrl = request.WebsiteUrl?.Trim();
        user.YearsOfExperience = request.YearsOfExperience;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToProfileDto(user);
    }

    public async Task<AdminSkillsOverviewStatsDto> GetOverviewStatsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var totalSkills = await _context.UserSkills.CountAsync(s => s.UserId == userId, cancellationToken);
        var totalExperiences = await _context.UserExperiences.CountAsync(e => e.UserId == userId, cancellationToken);
        var totalEducations = await _context.UserEducations.CountAsync(edu => edu.UserId == userId, cancellationToken);
        var highProficiencySkillsCount = await _context.UserSkills.CountAsync(s => s.UserId == userId && s.Proficiency >= 90, cancellationToken);

        return new AdminSkillsOverviewStatsDto
        {
            TotalSkills = totalSkills,
            TotalExperiences = totalExperiences,
            TotalEducations = totalEducations,
            HighProficiencySkillsCount = highProficiencySkillsCount
        };
    }

    // ==========================================
    // 2. Skills CRUD
    // ==========================================

    public async Task<List<AdminSkillDto>> GetSkillsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSkills
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Category)
            .Select(s => new AdminSkillDto
            {
                Id = s.Id,
                UserId = s.UserId,
                Name = s.Name,
                Category = s.Category,
                Proficiency = s.Proficiency,
                Icon = s.Icon,
                DisplayOrder = s.DisplayOrder,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminSkillDto> CreateSkillAsync(int userId, CreateSkillRequest request, CancellationToken cancellationToken = default)
    {
        var skill = new UserSkill
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Proficiency = Math.Clamp(request.Proficiency, 0, 100),
            Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserSkills.Add(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return new AdminSkillDto
        {
            Id = skill.Id,
            UserId = skill.UserId,
            Name = skill.Name,
            Category = skill.Category,
            Proficiency = skill.Proficiency,
            Icon = skill.Icon,
            DisplayOrder = skill.DisplayOrder,
            CreatedAt = skill.CreatedAt
        };
    }

    public async Task<AdminSkillDto?> UpdateSkillAsync(int userId, int skillId, UpdateSkillRequest request, CancellationToken cancellationToken = default)
    {
        var skill = await _context.UserSkills.FirstOrDefaultAsync(s => s.Id == skillId && s.UserId == userId, cancellationToken);
        if (skill is null)
        {
            return null;
        }

        skill.Name = request.Name.Trim();
        skill.Category = request.Category.Trim();
        skill.Proficiency = Math.Clamp(request.Proficiency, 0, 100);
        skill.Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim();
        skill.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync(cancellationToken);

        return new AdminSkillDto
        {
            Id = skill.Id,
            UserId = skill.UserId,
            Name = skill.Name,
            Category = skill.Category,
            Proficiency = skill.Proficiency,
            Icon = skill.Icon,
            DisplayOrder = skill.DisplayOrder,
            CreatedAt = skill.CreatedAt
        };
    }

    public async Task<bool> DeleteSkillAsync(int userId, int skillId, CancellationToken cancellationToken = default)
    {
        var skill = await _context.UserSkills.FirstOrDefaultAsync(s => s.Id == skillId && s.UserId == userId, cancellationToken);
        if (skill is null)
        {
            return false;
        }

        _context.UserSkills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ==========================================
    // 3. Experiences CRUD
    // ==========================================

    public async Task<List<AdminExperienceDto>> GetExperiencesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserExperiences
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.DisplayOrder)
            .ThenByDescending(e => e.IsCurrent)
            .Select(e => new AdminExperienceDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Role = e.Role,
                Company = e.Company,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent,
                Description = e.Description,
                Technologies = e.Technologies,
                DisplayOrder = e.DisplayOrder,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminExperienceDto> CreateExperienceAsync(int userId, CreateExperienceRequest request, CancellationToken cancellationToken = default)
    {
        var experience = new UserExperience
        {
            UserId = userId,
            Role = request.Role.Trim(),
            Company = request.Company.Trim(),
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
            StartDate = request.StartDate.Trim(),
            EndDate = request.IsCurrent ? null : (string.IsNullOrWhiteSpace(request.EndDate) ? null : request.EndDate.Trim()),
            IsCurrent = request.IsCurrent,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Technologies = string.IsNullOrWhiteSpace(request.Technologies) ? null : request.Technologies.Trim(),
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserExperiences.Add(experience);
        await _context.SaveChangesAsync(cancellationToken);

        return new AdminExperienceDto
        {
            Id = experience.Id,
            UserId = experience.UserId,
            Role = experience.Role,
            Company = experience.Company,
            Location = experience.Location,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrent = experience.IsCurrent,
            Description = experience.Description,
            Technologies = experience.Technologies,
            DisplayOrder = experience.DisplayOrder,
            CreatedAt = experience.CreatedAt
        };
    }

    public async Task<AdminExperienceDto?> UpdateExperienceAsync(int userId, int experienceId, UpdateExperienceRequest request, CancellationToken cancellationToken = default)
    {
        var experience = await _context.UserExperiences.FirstOrDefaultAsync(e => e.Id == experienceId && e.UserId == userId, cancellationToken);
        if (experience is null)
        {
            return null;
        }

        experience.Role = request.Role.Trim();
        experience.Company = request.Company.Trim();
        experience.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();
        experience.StartDate = request.StartDate.Trim();
        experience.EndDate = request.IsCurrent ? null : (string.IsNullOrWhiteSpace(request.EndDate) ? null : request.EndDate.Trim());
        experience.IsCurrent = request.IsCurrent;
        experience.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        experience.Technologies = string.IsNullOrWhiteSpace(request.Technologies) ? null : request.Technologies.Trim();
        experience.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync(cancellationToken);

        return new AdminExperienceDto
        {
            Id = experience.Id,
            UserId = experience.UserId,
            Role = experience.Role,
            Company = experience.Company,
            Location = experience.Location,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            IsCurrent = experience.IsCurrent,
            Description = experience.Description,
            Technologies = experience.Technologies,
            DisplayOrder = experience.DisplayOrder,
            CreatedAt = experience.CreatedAt
        };
    }

    public async Task<bool> DeleteExperienceAsync(int userId, int experienceId, CancellationToken cancellationToken = default)
    {
        var experience = await _context.UserExperiences.FirstOrDefaultAsync(e => e.Id == experienceId && e.UserId == userId, cancellationToken);
        if (experience is null)
        {
            return false;
        }

        _context.UserExperiences.Remove(experience);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // ==========================================
    // 4. Educations CRUD
    // ==========================================

    public async Task<List<AdminEducationDto>> GetEducationsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserEducations
            .AsNoTracking()
            .Where(edu => edu.UserId == userId)
            .OrderBy(edu => edu.DisplayOrder)
            .ThenByDescending(edu => edu.StartYear)
            .Select(edu => new AdminEducationDto
            {
                Id = edu.Id,
                UserId = edu.UserId,
                Institution = edu.Institution,
                Degree = edu.Degree,
                StartYear = edu.StartYear,
                EndYear = edu.EndYear,
                Description = edu.Description,
                DisplayOrder = edu.DisplayOrder,
                CreatedAt = edu.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminEducationDto> CreateEducationAsync(int userId, CreateEducationRequest request, CancellationToken cancellationToken = default)
    {
        var education = new UserEducation
        {
            UserId = userId,
            Institution = request.Institution.Trim(),
            Degree = request.Degree.Trim(),
            StartYear = string.IsNullOrWhiteSpace(request.StartYear) ? null : request.StartYear.Trim(),
            EndYear = string.IsNullOrWhiteSpace(request.EndYear) ? null : request.EndYear.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserEducations.Add(education);
        await _context.SaveChangesAsync(cancellationToken);

        return new AdminEducationDto
        {
            Id = education.Id,
            UserId = education.UserId,
            Institution = education.Institution,
            Degree = education.Degree,
            StartYear = education.StartYear,
            EndYear = education.EndYear,
            Description = education.Description,
            DisplayOrder = education.DisplayOrder,
            CreatedAt = education.CreatedAt
        };
    }

    public async Task<AdminEducationDto?> UpdateEducationAsync(int userId, int educationId, UpdateEducationRequest request, CancellationToken cancellationToken = default)
    {
        var education = await _context.UserEducations.FirstOrDefaultAsync(edu => edu.Id == educationId && edu.UserId == userId, cancellationToken);
        if (education is null)
        {
            return null;
        }

        education.Institution = request.Institution.Trim();
        education.Degree = request.Degree.Trim();
        education.StartYear = string.IsNullOrWhiteSpace(request.StartYear) ? null : request.StartYear.Trim();
        education.EndYear = string.IsNullOrWhiteSpace(request.EndYear) ? null : request.EndYear.Trim();
        education.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        education.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync(cancellationToken);

        return new AdminEducationDto
        {
            Id = education.Id,
            UserId = education.UserId,
            Institution = education.Institution,
            Degree = education.Degree,
            StartYear = education.StartYear,
            EndYear = education.EndYear,
            Description = education.Description,
            DisplayOrder = education.DisplayOrder,
            CreatedAt = education.CreatedAt
        };
    }

    public async Task<bool> DeleteEducationAsync(int userId, int educationId, CancellationToken cancellationToken = default)
    {
        var education = await _context.UserEducations.FirstOrDefaultAsync(edu => edu.Id == educationId && edu.UserId == userId, cancellationToken);
        if (education is null)
        {
            return false;
        }

        _context.UserEducations.Remove(education);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AdminProfileDto MapToProfileDto(User user)
    {
        return new AdminProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            JobTitle = user.JobTitle,
            Bio = user.Bio,
            AboutStory = user.AboutStory,
            Location = user.Location,
            Phone = user.Phone,
            CvUrl = user.CvUrl,
            GithubUrl = user.GithubUrl,
            LinkedinUrl = user.LinkedinUrl,
            FacebookUrl = user.FacebookUrl,
            TwitterUrl = user.TwitterUrl,
            WebsiteUrl = user.WebsiteUrl,
            YearsOfExperience = user.YearsOfExperience,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
