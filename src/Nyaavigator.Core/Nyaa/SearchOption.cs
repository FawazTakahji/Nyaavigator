namespace Nyaavigator.Core.Nyaa;

public class SearchOption
{
    public string Title { get; }
    public string Key { get; }

    public SearchOption(string title, string key)
    {
        Title = title;
        Key = key;
    }
}