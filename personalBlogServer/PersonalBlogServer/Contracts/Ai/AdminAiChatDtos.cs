using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Ai;

public class AdminChatMessageDto
{
    public string Role { get; set; } = "admin";
    public string Content { get; set; } = string.Empty;
}

public class AdminAiChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessageDto>? History { get; set; }
}

public class AdminAiChatResponse
{
    public bool Success { get; set; } = true;
    public string Reply { get; set; } = string.Empty;
    public string? Error { get; set; }
    public List<string> SuggestedFollowUps { get; set; } = new();
}
