using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? AvatarUrl { get; set; }

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

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

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<UserEducation> UserEducations { get; set; } = new List<UserEducation>();

    public virtual ICollection<UserExperience> UserExperiences { get; set; } = new List<UserExperience>();

    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
