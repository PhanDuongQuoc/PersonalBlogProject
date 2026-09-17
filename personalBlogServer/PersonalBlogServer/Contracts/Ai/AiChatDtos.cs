using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Ai;

public class ChatMessageDto
{
    public string Role { get; set; } = "user"; // "user" or "model"
    public string Content { get; set; } = string.Empty;
}

public class AiChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessageDto>? History { get; set; }
}

public class AiChatResponse
{
    public bool Success { get; set; } = true;
    public string Reply { get; set; } = string.Empty;
    public string? Error { get; set; }
    public List<string> SuggestedFollowUps { get; set; } = new();
}
