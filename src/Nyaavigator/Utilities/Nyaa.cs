using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Messages;
using Nyaavigator.Models;
using Notification = Nyaavigator.Models.Notification;

namespace Nyaavigator.Utilities;

internal static class Nyaa
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static async Task<(Stream?, string?)> GetTorrentFile(string request, CancellationToken token = default)
    {
        HttpClient httpClient = App.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("NyaaClient");

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(request, token);
            if (!response.IsSuccessStatusCode)
            {
                string message = $"A connection error occurred while downloading the file from \"{request}\".\n\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}";
                Logger.Error(message);
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content(message)
                    .ShowAndForget();
                return (null, null);
            }

            if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.ToString() != "application/x-bittorrent")
            {
                string message = $"Couldn't download the file from \"{request}\" because it's not a torrent file.";
                Logger.Error(message);
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content(message)
                    .ShowAndForget();
                return (null, null);
            }

            Stream torrentStream = await response.Content.ReadAsStreamAsync();
            string? name = response.Content.Headers.ContentDisposition?.FileNameStar;
            return (torrentStream, name);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while downloading the file from \"{request}\".";
            Logger.Error(ex, message);
            Dialog.Create()
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
            return (null, null);
        }
    }

    public static async Task<(List<Torrent>, List<PageButton>, string)> Search(string request, CancellationToken token = default)
    {
        string content = await FetchContent(request, token);

        if (string.IsNullOrEmpty(content))
        {
            Logger.Error($"Response from \"{request}\" doesn't contain any content.");
            new Notification("Search Failed", "Response doesn't contain any content.", NotificationType.Error).Send();
            return ([], [], string.Empty);
        }

        HtmlDocument document = new();
        document.LoadHtml(content);

        HtmlNodeCollection headers = document.DocumentNode.SelectNodes("//h1 | //h2 | //h3 | //h4 | //h5 | //h6");
        if (headers != null && Html.NoResults(headers))
        {
            Dialog.Create()
                .Type(DialogType.Info)
                .Content("No Results Found")
                .ShowAndForget();
            return ([], [], string.Empty);
        }

        List<Torrent> torrents = [];
        HtmlNodeCollection torrentNodes = document.DocumentNode.SelectNodes("//table[(contains(@class, 'torrent-list'))]/tbody/tr");
        if (torrentNodes != null)
        {
            foreach (HtmlNode node in torrentNodes)
            {
                Torrent torrent = new();
                torrent.ParseTorrent(node);
                torrents.Add(torrent);
            }
        }
        else
        {
            Logger.Error($"Failed to get the results table from \"{request}\".");
            Dialog.Create()
                .Type(DialogType.Error)
                .Content("Failed to get the results.")
                .ShowAndForget();
        }

        List<PageButton> pages = Html.ParsePageButtons(document);
        if (pages.Count <= 0)
            Logger.Warn($"Failed to get the buttons from \"{request}\", Maybe there's only one page.");

        string resultsString = Html.GetResultsString(document) ?? Html.GetUserTorrentsString(headers) ?? string.Empty;

        return (torrents, pages, resultsString);
    }

    public static async Task<TorrentInfo?> GetTorrentInfo(string request, CancellationToken token)
    {
        string content = await FetchContent(request, token);

        if (string.IsNullOrEmpty(content))
        {
            WeakReferenceMessenger.Default.Send(new InfoViewMessage("Response doesn't contain any content."));
            return null;
        }

        HtmlDocument document = new();
        document.LoadHtml(content);

        HtmlNode infoPanel = document.DocumentNode.SelectSingleNode(
            "//div[(contains(@class, 'panel-default') or contains(@class, 'panel-success') or contains(@class, 'panel-danger')) and .//text()[contains(., 'Info hash:')]]");
        HtmlNode? infoPanelTitle = infoPanel?.SelectSingleNode(".//h3[(contains(@class, 'panel-title'))]");
        HtmlNode? infoPanelBody = infoPanel?.SelectSingleNode(".//div[(contains(@class, 'panel-body'))]");

        HtmlNode fileList = document.DocumentNode.SelectSingleNode("//div[(contains(@class, 'torrent-file-list'))]/ul");
        HtmlNode description = document.DocumentNode.SelectSingleNode("//div[(contains(@id, 'torrent-description'))]");

        HtmlNode commentsPanel = document.DocumentNode.SelectSingleNode("//div[(contains(@id, 'comments')) and (contains(@class, 'panel-default'))]");
        HtmlNode? commentsPanelTitle = commentsPanel?.SelectSingleNode(".//h3[(contains(@class, 'panel-title'))]");
        HtmlNodeCollection? commentNodes = commentsPanel?.SelectNodes(".//div[(contains(@class, 'comment-panel'))]");

        if (infoPanelTitle == null && infoPanelBody == null && fileList == null && description == null && commentNodes == null)
        {
            Logger.Error($"Failed to get the info of the torrent from \"{request}\".");
            return null;
        }

        TorrentInfo torrentInfo = new();

        if (infoPanelTitle != null)
            torrentInfo.Name = WebUtility.HtmlDecode(infoPanelTitle.InnerText).Trim();
        else
            Logger.Error($"Failed to get the name of the torrent from \"{request}\".");

        if (infoPanelBody != null)
            torrentInfo.ParseTorrentInfo(infoPanelBody);
        else
            Logger.Error($"Failed to get the info of the torrent from \"{request}\".");

        if (fileList != null)
            torrentInfo.Items.AddRange(Html.ParseTorrentItems(fileList));
        else
            Logger.Error($"Failed to get the items of the torrent from \"{request}\".");

        if (description != null)
            torrentInfo.Description = WebUtility.HtmlDecode(description.InnerText);
        else
            Logger.Error($"Failed to get the description of the torrent from \"{request}\".");

        if (commentNodes != null)
            torrentInfo.ParseComments(commentNodes);
        else if (!Html.NoComments(commentsPanelTitle))
            Logger.Warn($"Failed to get the comments from \"{request}\".");

        return torrentInfo;
    }

    private static async Task<string> FetchContent(string request, CancellationToken token = default)
    {
        HttpClient httpClient = App.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("NyaaClient");

        string content = string.Empty;
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(request, token);
            if (!response.IsSuccessStatusCode)
            {
                Logger.Error($"A connection error occurred while fetching content from \"{request}\".\n\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}");
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content($"A connection error occurred while fetching content.\n\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}")
                    .ShowAndForget();
            }
            else
                content = await response.Content.ReadAsStringAsync(token);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"An error occurred while fetching content from \"{request}\".");
            Dialog.Create()
                .Type(DialogType.Error)
                .Content("An error occurred while fetching content.")
                .ShowAndForget();
        }

        return content;
    }
}