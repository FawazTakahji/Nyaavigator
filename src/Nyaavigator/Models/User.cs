namespace Nyaavigator.Models;

public class User(string? name = null, string? href = null)
{
    public string? Name { get; set; } = name;
    public string? Href { get; set; } = href;
    public bool IsTrusted { get; set; }
    public bool IsBanned { get; set; }
    public bool IsAdmin { get; set; }
    public string? Link => string.IsNullOrEmpty(Href) ? null : $"https://nyaa.si/{Href.TrimStart('/')}";
}