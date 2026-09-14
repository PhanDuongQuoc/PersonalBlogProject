using System;
using System.Collections.Generic;

namespace PersonalBlogServer.Models;

public partial class PostImage
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public string? AltText { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Post Post { get; set; } = null!;
}
