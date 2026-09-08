using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class UserSkill
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int Proficiency { get; set; }

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
