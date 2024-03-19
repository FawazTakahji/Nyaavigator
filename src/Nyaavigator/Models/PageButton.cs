namespace Nyaavigator.Models;

public class PageButton(object? content, string? href, bool isEnabled, bool isActive)
{
    public object? Content { get; } = content;
    public string? Href { get; } = href;
    public bool IsEnabled { get; } = isEnabled;
    public bool IsActive { get; } = isActive;
}