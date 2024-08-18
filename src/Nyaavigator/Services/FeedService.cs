using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Nyaavigator.Models;
using FeedsUtils = Nyaavigator.Utilities.Feeds;
using ErrorOr;
using NLog;
using Notification = Nyaavigator.Models.Notification;

namespace Nyaavigator.Services;

public class FeedService
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    public ObservableCollection<Feed> Feeds { get; set; }

    public FeedService()
    {
        Feeds = LoadFeeds();
        Feeds.CollectionChanged += FeedsChanged;
    }

    public async Task<ErrorOr<Success>> AddFeed(string user)
    {
        if (Feeds.Any(f => f.User.Equals(user, StringComparison.OrdinalIgnoreCase)))
            return new Success();

        Feed feed = new(user);
        ErrorOr<Success> result = await feed.GetLatestRelease();
        if (result.IsError)
        {
            return result.FirstError;
        }

        Feeds.Add(feed);
        return new Success();
    }

    public void RemoveFeed(string user)
    {
        Feed? feed = Feeds.FirstOrDefault(f => f.User.Equals(user, StringComparison.OrdinalIgnoreCase));
        if (feed is null)
            return;

        Feeds.Remove(feed);
    }

    public bool IsUserFollowed(string user)
    {
        return Feeds.Any(f => f.User.Equals(user, StringComparison.OrdinalIgnoreCase));
    }

    private void FeedsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SaveFeeds();
    }

    public void SaveFeeds()
    {
        ErrorOr<Success> result = FeedsUtils.Save(Feeds.ToList());
        if (result.IsError)
        {
            new Notification("Error", $"Failed to save the feeds: {result.FirstError.Description}.", NotificationType.Error).Send();

            if (result.FirstError.Metadata?["Exception"] is Exception ex)
                _logger.Error(ex, $"Failed to save the feeds: {result.FirstError.Description}.");
            else
                _logger.Error($"Failed to save the feeds: {result.FirstError.Description}.");
        }
    }

    private ObservableCollection<Feed> LoadFeeds()
    {
        ErrorOr<List<Feed>> feeds = FeedsUtils.Load();
        if (feeds.IsError)
        {
            ObservableCollection<Feed> feedsCollection = [];

            if (feeds.FirstError.Code == "FileNotFound")
            {
                ErrorOr<Success> result = FeedsUtils.Save(feedsCollection.ToList());
                if (result.IsError && result.FirstError.Metadata?["Exception"] is Exception saveEx)
                    _logger.Error(saveEx, $"Failed to save the feeds: {result.FirstError.Description}.");

                return feedsCollection;
            }

            new Notification("Error", $"Failed to load the feeds: {feeds.FirstError.Description}.", NotificationType.Error).Send();
            if (feeds.FirstError.Metadata?["Exception"] is Exception ex)
            {
                _logger.Error(ex, $"Failed to load the feeds: {feeds.FirstError.Description}.");
            }
            else
            {
                _logger.Error($"Failed to load the feeds: {feeds.FirstError.Description}.");
            }

            return feedsCollection;
        }

        return new ObservableCollection<Feed>(feeds.Value);
    }

#if DEBUG
    public FeedService(ObservableCollection<Feed> feeds)
    {
        Feeds = feeds;
    }
#endif
}