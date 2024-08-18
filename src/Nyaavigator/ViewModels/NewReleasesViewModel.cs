using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Nyaavigator.Models;

namespace Nyaavigator.ViewModels;

public partial class NewReleasesViewModel : ObservableObject
{
    public List<NewReleases> Releases { get; }

    [ObservableProperty]
    private string _filterText = string.Empty;

    public Predicate<object> Filter
    {
        get
        {
            return (obj) =>
            {
                if (string.IsNullOrEmpty(FilterText))
                    return true;

                return obj is NewReleases newReleases && newReleases.User.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
            };
        }
    }

    public NewReleasesViewModel(List<NewReleases> releases)
    {
        Releases = releases;
    }

    partial void OnFilterTextChanged(string value)
    {
        OnPropertyChanged(nameof(Filter));
    }

#if DEBUG
    public NewReleasesViewModel()
    {
        Releases =
        [
            new NewReleases("User2",
                [
                    new RssRelease("Very cool anime 1", "https://www.example.com"),
                    new RssRelease("Very cool anime 2", "https://www.example.com")
                ]),
            new NewReleases("User1",
                [
                    new RssRelease("Very cool anime 3", "https://www.example.com"),
                    new RssRelease("Very cool anime 4", "https://www.example.com")
                ]),
            new NewReleases("User3",
                [
                    new RssRelease("Very cool anime 5", "https://www.example.com"),
                    new RssRelease("Very cool anime 6", "https://www.example.com")
                ]),
            new NewReleases("User4",
                [
                    new RssRelease("Very cool anime 7", "https://www.example.com"),
                    new RssRelease("Very cool anime 8", "https://www.example.com")
                ])
        ];
    }
#endif
}