using System.Collections.Generic;

namespace PersonalBlogServer.Contracts.Ai;

public class PromptMetadata
{
    public string BotName { get; set; } = "PDQ AI Assistant";
    public string Version { get; set; } = "1.0.0";
    public string? Description { get; set; }
}

public class PromptParameters
{
    public double Temperature { get; set; } = 0.7;
    public double TopP { get; set; } = 0.9;
    public int MaxOutputTokens { get; set; } = 1200;
}

public class PromptPersona
{
    public string Role { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
    public string PrimaryLanguage { get; set; } = "Tiếng Việt";
    public string FormatStyle { get; set; } = string.Empty;
}

public class SystemPromptConfig
{
    public PromptMetadata Metadata { get; set; } = new();
    public PromptParameters Parameters { get; set; } = new();
    public PromptPersona Persona { get; set; } = new();
    public List<string> Rules { get; set; } = new();
    public string Template { get; set; } = string.Empty;
}
