using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonalBlogServer.Contracts.Admin;

namespace PersonalBlogServer.Services.Admin;

public interface IAdminProfileService
{
    // Profile
    Task<AdminProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<AdminProfileDto?> UpdateProfileAsync(int userId, UpdateAdminProfileRequest request, CancellationToken cancellationToken = default);
    Task<AdminSkillsOverviewStatsDto> GetOverviewStatsAsync(int userId, CancellationToken cancellationToken = default);

    // Skills
    Task<List<AdminSkillDto>> GetSkillsAsync(int userId, CancellationToken cancellationToken = default);
    Task<AdminSkillDto> CreateSkillAsync(int userId, CreateSkillRequest request, CancellationToken cancellationToken = default);
    Task<AdminSkillDto?> UpdateSkillAsync(int userId, int skillId, UpdateSkillRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteSkillAsync(int userId, int skillId, CancellationToken cancellationToken = default);

    // Experiences
    Task<List<AdminExperienceDto>> GetExperiencesAsync(int userId, CancellationToken cancellationToken = default);
    Task<AdminExperienceDto> CreateExperienceAsync(int userId, CreateExperienceRequest request, CancellationToken cancellationToken = default);
    Task<AdminExperienceDto?> UpdateExperienceAsync(int userId, int experienceId, UpdateExperienceRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteExperienceAsync(int userId, int experienceId, CancellationToken cancellationToken = default);

    // Educations
    Task<List<AdminEducationDto>> GetEducationsAsync(int userId, CancellationToken cancellationToken = default);
    Task<AdminEducationDto> CreateEducationAsync(int userId, CreateEducationRequest request, CancellationToken cancellationToken = default);
    Task<AdminEducationDto?> UpdateEducationAsync(int userId, int educationId, UpdateEducationRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteEducationAsync(int userId, int educationId, CancellationToken cancellationToken = default);
}
