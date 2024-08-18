namespace Nyaavigator.Models;

public class RssRelease
{
    public string Title { get; }
    public string Link { get; }

    public RssRelease(string title, string link)
    {
        Title = title;
        Link = link;
    }
}