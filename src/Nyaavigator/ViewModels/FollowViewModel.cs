using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErrorOr;
using NLog;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Models;
using Nyaavigator.Services;
using Nyaavigator.Views;
using Notification = Nyaavigator.Models.Notification;
using Nyaavigator.Utilities;

namespace Nyaavigator.ViewModels;

public partial class FollowViewModel : ObservableObject
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public FeedService FeedService { get; }

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(FollowUserCommand))]
    private string _newUserText = string.Empty;
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

                return obj is Feed feed && feed.User.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
            };
        }
    }

    public FollowViewModel(FeedService feedService)
    {
        FeedService = feedService;
        CheckAllReleasesCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CheckAllReleasesCommand.IsRunning))
            {
                CheckAllReleasesCommand.NotifyCanExecuteChanged();
                CheckUserReleasesCommand.NotifyCanExecuteChanged();
            }
        };
        CheckUserReleasesCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CheckUserReleasesCommand.IsRunning))
            {
                CheckAllReleasesCommand.NotifyCanExecuteChanged();
                CheckUserReleasesCommand.NotifyCanExecuteChanged();
            }
        };
    }

    partial void OnFilterTextChanged(string value)
    {
        OnPropertyChanged(nameof(Filter));
    }

    [RelayCommand]
    private static async Task ShowMoreInfo(string? link)
    {
        if (string.IsNullOrEmpty(link))
        {
            new Notification("Torrent link is invalid.", type:NotificationType.Error).Send();
            return;
        }

        TorrentInfoViewModel viewModel = new(link);
        await new TorrentInfoView(viewModel).ShowAsync();
    }

    [RelayCommand(CanExecute = nameof(CanFollowUser))]
    private async Task FollowUser()
    {
        ErrorOr<Success> result = await FeedService.AddFeed(NewUserText);
        if (result.IsError)
        {
            Dialog.Create()
                .Type(DialogType.Error)
                .Content($"Failed to get the latest release for \"{NewUserText}\" while adding the user to the feed: {result.FirstError.Description}.")
                .ShowAndForget();

            if (result.FirstError.Metadata?["Exception"] is Exception ex)
                _logger.Error(ex, $"Failed to get the latest release for {NewUserText}: {result.FirstError.Description}.");
            else
                _logger.Error($"Failed to get the latest release for {NewUserText}: {result.FirstError.Description}.");
        }
    }

    [RelayCommand]
    private void UnfollowUser(string username)
    {
        FeedService.RemoveFeed(username);
    }

    [RelayCommand(CanExecute = nameof(CanCheckReleases))]
    private async Task CheckUserReleases(string username)
    {
        Feed? feed = FeedService.Feeds.FirstOrDefault(f => f.User == username);

        if (feed is null)
        {
            Dialog.Create()
                .Type(DialogType.Error)
                .Content($"Couldn't find the user \"{username}\" in the following feeds.")
                .ShowAndForget();
            return;
        }

        ErrorOr<List<RssRelease>> releases = await Rss.GetUserReleases(username);
        if (releases.IsError)
        {
            Dialog.Create()
                .Type(DialogType.Error)
                .Content($"Failed to get the releases for \"{username}\": {releases.FirstError.Description}.")
                .ShowAndForget();

            if (releases.FirstError.Metadata?["Exception"] is Exception ex)
                _logger.Error(ex,
                    $"Failed to get the releases for {username}: {releases.FirstError.Description}.");
            else
                _logger.Error($"Failed to get the releases for {username}: {releases.FirstError.Description}.");
            return;
        }

        if (releases.Value.Count <= 0)
        {
            Dialog.Create()
                .Type(DialogType.Info)
                .Content($"No releases found for \"{username}\".")
                .ShowAndForget();
            return;
        }

        if (feed.LatestRelease is null)
        {
            feed.LatestRelease = releases.Value.First();
            FeedService.SaveFeeds();
            await new NewReleasesView
            {
                DataContext = new NewReleasesViewModel([new NewReleases(feed.User, releases.Value)])
            }.ShowAsync();
            return;
        }

        List<RssRelease> newReleases = releases.Value
            .TakeWhile(r => r.Link != feed.LatestRelease.Link)
            .ToList();

        if (newReleases.Count <= 0)
        {
            Dialog.Create()
                .Type(DialogType.Info)
                .Content($"No new releases found for \"{username}\".")
                .ShowAndForget();
            return;
        }

        feed.LatestRelease = newReleases.First();
        FeedService.SaveFeeds();

        await new NewReleasesView
        {
            DataContext = new NewReleasesViewModel([new NewReleases(feed.User, newReleases)])
        }.ShowAsync();
    }

    [RelayCommand(CanExecute = nameof(CanCheckReleases))]
    private async Task CheckAllReleases()
    {
        List<NewReleases> newReleases = [];
        Feed[] feeds = FeedService.Feeds.ToArray();

        foreach (Feed feed in feeds)
        {
            ErrorOr<List<RssRelease>> releases = await Rss.GetUserReleases(feed.User);
            if (releases.IsError)
            {
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content($"Failed to get the releases for \"{feed.User}\": {releases.FirstError.Description}.")
                    .ShowAndForget();

                if (releases.FirstError.Metadata?["Exception"] is Exception ex)
                    _logger.Error(ex,
                        $"Failed to get the releases for {feed.User}: {releases.FirstError.Description}.");
                else
                    _logger.Error($"Failed to get the releases for {feed.User}: {releases.FirstError.Description}.");
                continue;
            }

            if (releases.Value.Count <= 0)
                continue;

            List<RssRelease> userNewReleases = releases.Value
                .TakeWhile(r => r.Link != feed.LatestRelease?.Link)
                .ToList();

            if (userNewReleases.Count <= 0)
                continue;

            newReleases.Add(new NewReleases(feed.User, userNewReleases));

            Feed? originalFeed = FeedService.Feeds.FirstOrDefault(f => f.User == feed.User);
            if (originalFeed is not null)
                originalFeed.LatestRelease = userNewReleases.First();
        }

        FeedService.SaveFeeds();

        if (newReleases.Count <= 0)
        {
            Dialog.Create()
                .Type(DialogType.Info)
                .Content("No new releases found.")
                .ShowAndForget();
            return;
        }

        await new NewReleasesView
        {
            DataContext = new NewReleasesViewModel(newReleases)
        }.ShowAsync();
    }

    private bool CanFollowUser() => !string.IsNullOrEmpty(NewUserText);
    private bool CanCheckReleases() => !CheckUserReleasesCommand.IsRunning && !CheckAllReleasesCommand.IsRunning;

#if DEBUG
    public FollowViewModel()
    {
        ObservableCollection<Feed> feeds =
        [
            new Feed("User2")
            {
                LatestRelease = new RssRelease("Very cool anime", "https://www.example.com")
            },
            new Feed("User1")
            {
                LatestRelease = new RssRelease("Very cool anime", "https://www.example.com")
            },
            new Feed("User3")
            {
                LatestRelease = new RssRelease("Very cool anime", "https://www.example.com")
            },
            new Feed("User4")
        ];
        FeedService = new FeedService(feeds);
    }
#endif
}