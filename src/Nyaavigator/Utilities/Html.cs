using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Material.Icons;
using Material.Icons.Avalonia;
using Nyaavigator.Extensions;
using Nyaavigator.Models;

namespace Nyaavigator.Utilities;

internal static class Html
{
    public static bool NoResults(HtmlNodeCollection headers)
    {
        return headers.Any(header => header.InnerText.Contains("no results", StringComparison.OrdinalIgnoreCase));
    }

    public static string? GetResultsString(HtmlDocument document)
    {
        HtmlNode textNode = document.DocumentNode.SelectSingleNode("//div[@class='pagination-page-info']");
        if (textNode == null)
            return null;

        Regex regex = new(@"displaying\s*results\s*(\d*)-(\d*)\s*out\s*of\s*(\d*)\s*results", RegexOptions.IgnoreCase);
        Match match = regex.Match(textNode.InnerText);
        return match.Success
            ? $"Displaying results {match.Groups[1].Value.Trim()}-{match.Groups[2].Value.Trim()} out of {match.Groups[3].Value.Trim()} results."
            : null;
    }

    public static string? GetUserTorrentsString(HtmlNodeCollection? headers)
    {
        if (headers == null)
            return null;

        Regex regex = new(@"browsing\s*(.*)\s*torrents\s*\((\d*)\)", RegexOptions.IgnoreCase);

        foreach (HtmlNode header in headers)
        {
            Match match = regex.Match(header.InnerText);
            if (match.Success)
                return $"Browsing {match.Groups[1].Value.Trim()} torrents ({match.Groups[2].Value.Trim()})";
        }

        return null;
    }

    public static void ParseTorrent(this Torrent torrent, HtmlNode node)
    {
        HtmlNode categoryNode = node.SelectSingleNode("./td[1]/a");
        HtmlNode nameNode = node.SelectSingleNode("./td[2]/a[not(contains(@class, 'comments'))]");
        HtmlNode commentsNode = node.SelectSingleNode("./td[2]/a[(contains(@class, 'comments'))]");
        HtmlNode downloadNode = node.SelectSingleNode("./td[3]/a[i[(contains(@class, 'fa-download'))]]");
        HtmlNode magnetNode = node.SelectSingleNode("./td[3]/a[i[(contains(@class, 'fa-magnet'))]]");
        HtmlNode sizeNode = node.SelectSingleNode("./td[4]");
        HtmlNode dateNode = node.SelectSingleNode("./td[5]");
        HtmlNode seedersNode = node.SelectSingleNode("./td[6]");
        HtmlNode leechersNode = node.SelectSingleNode("./td[7]");
        HtmlNode completedDownloadsNode = node.SelectSingleNode("./td[8]");

        if (categoryNode?.GetAttributeValue("href", null) is { } categoryHref)
        {
            Match match = Regex.Match(categoryHref, @"\/\?c=(\d{1}_\d{1})");
            if (match.Success)
                torrent.Category = match.Groups[1].Value;
        }

        if (nameNode != null)
        {
            string? title = nameNode.GetAttributeValue("title", null);
            torrent.Name = WebUtility.HtmlDecode(title ?? nameNode.InnerText);

            torrent.Href = nameNode.GetAttributeValue("href", null);
        }

        if (commentsNode != null)
        {
            if (commentsNode.GetAttributeValue("title", null) is { } title)
                torrent.Comments = WebUtility.HtmlDecode(title).TryGetBeginningInt();
            else if (int.TryParse(WebUtility.HtmlDecode(commentsNode.InnerText), out int comments))
                torrent.Comments = comments;
        }

        torrent.DownloadHref = downloadNode?.GetAttributeValue("href", null);
        torrent.Magnet = magnetNode?.GetAttributeValue("href", null);

        if (dateNode?.GetAttributeValue("data-timestamp", null) is { } timeStamp && int.TryParse(timeStamp, out int timeStampInt))
            torrent.Date = DateTimeOffset.FromUnixTimeSeconds(timeStampInt).DateTime.ToLocalTime();

        torrent.Size = sizeNode?.InnerText;
        torrent.Seeders = seedersNode?.InnerText;
        torrent.Leechers = leechersNode?.InnerText;
        torrent.Downloads = completedDownloadsNode?.InnerText;
    }

    public static List<PageButton> ParsePageButtons(HtmlDocument document)
    {
        List<PageButton> pages = [];

        HtmlNodeCollection pageNodes = document.DocumentNode.SelectNodes("//ul[(contains(@class, 'pagination'))]/li");
        if (pageNodes != null)
        {
            foreach (HtmlNode pageNode in pageNodes)
            {
                string[] classes = pageNode.GetAttributeValue("class", string.Empty).Split(" ");

                object? pageContent;
                if (classes.Contains("previous") || pageNode.InnerText == "&laquo;")
                    pageContent = new MaterialIcon { Kind = MaterialIconKind.ChevronDoubleLeft };
                else if (classes.Contains("next") || pageNode.InnerText == "&raquo;")
                    pageContent = new MaterialIcon { Kind = MaterialIconKind.ChevronDoubleRight };
                else
                    pageContent = pageNode.InnerText.Length > 0 && char.IsDigit(pageNode.InnerText[0])
                        ? pageNode.InnerText.GetBeginningInt()
                        : pageNode.InnerText;

                PageButton pageButton = new(pageContent, pageNode.ChildNodes[0]?.Attributes["href"]?.DeEntitizeValue,
                    !classes.Contains("disabled") && !classes.Contains("active"), classes.Contains("active"));
                pages.Add(pageButton);
            }
        }

        return pages;
    }

    public static void ParseTorrentInfo(this TorrentInfo torrentInfo, HtmlNode panelBody)
    {
        HtmlNode categoryNode = panelBody.SelectSingleNode(".//div[text()='Category:']/following-sibling::div/a[2]");
        HtmlNode submitterNode = panelBody.SelectSingleNode(".//div[text()='Submitter:']/following-sibling::div/a");
        HtmlNode informationNode = panelBody.SelectSingleNode(".//div[text()='Information:']/following-sibling::div/a");
        HtmlNode sizeNode = panelBody.SelectSingleNode(".//div[text()='File size:']/following-sibling::div");
        HtmlNode hashNode = panelBody.SelectSingleNode(".//div[text()='Info hash:']/following-sibling::div/kbd");
        HtmlNode dateNode = panelBody.SelectSingleNode(".//div[text()='Date:']/following-sibling::div");
        HtmlNode seedersNode = panelBody.SelectSingleNode(".//div[text()='Seeders:']/following-sibling::div/span");
        HtmlNode leechersNode = panelBody.SelectSingleNode(".//div[text()='Leechers:']/following-sibling::div/span");
        HtmlNode completedNode = panelBody.SelectSingleNode(".//div[text()='Completed:']/following-sibling::div");

        if (categoryNode?.GetAttributeValue("href", null) is { } categoryHref)
        {
            Match match = Regex.Match(categoryHref, @"\/\?c=(\d{1}_\d{1})");
            if (match.Success)
                torrentInfo.Category = match.Groups[1].Value;
        }

        if (submitterNode != null)
        {
            torrentInfo.Submitter.Name = WebUtility.HtmlDecode(submitterNode.InnerText);
            torrentInfo.Submitter.Href = submitterNode.GetAttributeValue("href", null);
        }
        else
        {
            submitterNode = panelBody.SelectSingleNode(".//div[text()='Submitter:']/following-sibling::div");
            if (submitterNode != null && submitterNode.InnerText.Contains("anonymous", StringComparison.OrdinalIgnoreCase))
                torrentInfo.Submitter.Name = "Anonymous";
        }

        if (informationNode != null)
            torrentInfo.Information = informationNode.GetAttributeValue("href", null);
        else
        {
            informationNode = panelBody.SelectSingleNode(".//div[text()='Information:']/following-sibling::div");
            if (informationNode != null)
                torrentInfo.Information = informationNode.InnerText.Contains("no information", StringComparison.OrdinalIgnoreCase)
                        ? "No Information"
                        : WebUtility.HtmlDecode(informationNode.InnerText).Trim();
        }

        if (hashNode != null)
            torrentInfo.Hash = WebUtility.HtmlDecode(hashNode.InnerText);

        if (dateNode?.GetAttributeValue("data-timestamp", null) is { } timeStamp && int.TryParse(timeStamp, out int timeStampInt))
            torrentInfo.Date = DateTimeOffset.FromUnixTimeSeconds(timeStampInt).DateTime.ToLocalTime();

        torrentInfo.Size = sizeNode?.InnerText;
        torrentInfo.Seeders = seedersNode?.InnerText;
        torrentInfo.Leechers = leechersNode?.InnerText;
        torrentInfo.Downloads = completedNode?.InnerText;
    }

    public static List<ListItem> ParseTorrentItems(HtmlNode list)
    {
        List<ListItem> items = [];

        foreach (HtmlNode node in list.ChildNodes)
        {
            if (node.Name != "li")
                continue;

            foreach (HtmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "i" when childNode.AttributeContainsInsensitive("class", "file"):
                        items.Add(ParseFile(childNode));
                        break;
                    case "ul":
                        if (ParseFolder(childNode) is { } folder)
                            items.Add(folder);
                        break;
                }
            }
        }

        return items;
    }

    private static ListItem ParseFile(HtmlNode node)
    {
        HtmlNode nameNode = node.SelectSingleNode("./following-sibling::text()");
        HtmlNode sizeNode = node.SelectSingleNode("./following-sibling::span[(contains(@class, 'size'))]");

        ListItem file = new()
        {
            Name = nameNode != null ? WebUtility.HtmlDecode(nameNode.InnerText).Trim() : null,
            Size = sizeNode != null ? WebUtility.HtmlDecode(sizeNode.InnerText).TrimStart('(').TrimEnd(')') : null
        };
        return file;
    }

    private static ListItem? ParseFolder(HtmlNode node)
    {
        HtmlNode prevSibling = node.SelectSingleNode("./preceding-sibling::*");
        if (prevSibling == null || !prevSibling.AttributeContainsInsensitive("class", "folder"))
            return null;

        ListItem folder = new()
        {
            Name = WebUtility.HtmlDecode(prevSibling.InnerText.Trim()),
            Children = ParseTorrentItems(node),
            IsFolder = true
        };
        return folder;
    }

    public static bool NoComments(HtmlNode? node)
    {
        return node != null && node.InnerText.Contains("Comments - 0");
    }

    public static void ParseComments(this TorrentInfo torrentInfo, HtmlNodeCollection commentNodes)
    {
        foreach (HtmlNode node in commentNodes)
        {
            HtmlNode userNode = node.SelectSingleNode(".//a[(contains(@title, 'User')) or (contains(@title, 'Trusted')) or (contains(@title, 'Administrator'))]");
            HtmlNode dateNode = node.SelectSingleNode(".//*[@data-timestamp and not(contains(text(), '(edited)'))]");
            HtmlNode editedDateNode = node.SelectSingleNode(".//*[@data-timestamp and contains(text(), '(edited)')]");
            HtmlNode textNode = node.SelectSingleNode(".//div[(contains(@class, 'comment-content'))]");
            if (userNode == null && dateNode == null && editedDateNode == null && textNode == null)
                continue;

            Comment comment = new();

            if (userNode != null)
            {
                comment.User.Name = WebUtility.HtmlDecode(userNode.InnerText);
                comment.User.Href = userNode.GetAttributeValue("href", null);
                comment.IsUploader = userNode.NextSibling is { } sibling && sibling.InnerText.Contains("(uploader)");

                if (userNode.GetAttributeValue("title", null) is {} userNodeTitle)
                {
                    comment.User.IsTrusted = userNodeTitle.Contains("Trusted");
                    comment.User.IsBanned = userNodeTitle.Contains("BANNED");
                    comment.User.IsAdmin = userNodeTitle.Contains("Administrator");
                }
            }

            if (dateNode?.GetAttributeValue("data-timestamp", null) is { } timeStamp && int.TryParse(timeStamp, out int timeStampInt))
                comment.Date = DateTimeOffset.FromUnixTimeSeconds(timeStampInt).DateTime.ToLocalTime();

            if (editedDateNode != null)
            {
                if (editedDateNode.GetAttributeValue("data-timestamp", null) is { } editedTimeStamp &&
                    int.TryParse(editedTimeStamp.RemoveEndingString(".0"), out int editedTimeStampInt))
                {
                    comment.EditedDate = DateTimeOffset.FromUnixTimeSeconds(editedTimeStampInt).DateTime.ToLocalTime();
                }

                comment.IsEdited = true;
            }

            if (textNode != null)
                comment.Text = WebUtility.HtmlDecode(textNode.InnerText);

            torrentInfo.Comments.Add(comment);
        }
    }
}