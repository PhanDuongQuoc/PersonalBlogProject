using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class UserEducation
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

    public virtual User User { get; set; } = null!;
}
