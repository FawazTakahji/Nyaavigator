using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ErrorOr;

namespace Nyaavigator.Models;

public partial class Feed : ObservableObject
{
    public string User { get; }

    [ObservableProperty] [property: JsonPropertyName("Latest Release")]
    private RssRelease? _latestRelease;

    public Feed(string user)
    {
        User = user;
    }

    public async Task<ErrorOr<Success>> GetLatestRelease()
    {
        ErrorOr<List<RssRelease>> releases = await Utilities.Rss.GetUserReleases(User);
        if (releases.IsError)
            return releases.FirstError;

        LatestRelease = releases.Value.First();
        return new Success();
    }
}