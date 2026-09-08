using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class UserExperience
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

    public virtual User User { get; set; } = null!;
}
