using System.Collections.Generic;

namespace Nyaavigator.Models;

public class NewReleases
{
    public string User { get; }
    public List<RssRelease> Releases { get; }

    public NewReleases(string user, List<RssRelease> releases)
    {
        User = user;
        Releases = releases;
    }
}