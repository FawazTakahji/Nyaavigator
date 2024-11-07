using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ErrorOr;
using Nyaavigator.Enums;
using Nyaavigator.Models;
using QBittorrent.Client;

namespace Nyaavigator.Utilities;

public static class QBittorrent
{
    public static async Task<ErrorOr<Success>> AddTorrents(IEnumerable<string> torrents, QBittorrentSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Host))
            return Error.Failure(description: "Host in not valid");
        if (string.IsNullOrEmpty(settings.Username))
            return Error.Failure(description: "Username in not valid");

        List<Uri> torrentUrls = [];
        foreach (string torrent in torrents)
        {
            if (Uri.TryCreate(torrent, UriKind.Absolute, out Uri? uri))
                torrentUrls.Add(uri);
            else
                return Error.Failure(description: $"The following magnet is not valid \"{torrent}\"");
        }

        string protocol = settings.Protocol switch
        {
            Protocol.Http => "http",
            Protocol.Https => "https"
        };

        QBittorrentClient client;
        try
        {
            client = new(new Uri($"{protocol}://{settings.Host}:{settings.Port}"));
            await client.LoginAsync(settings.Username, settings.Password);
        }
        catch (HttpRequestException ex)
        {
            return ex.HttpRequestError switch
            {
                HttpRequestError.NameResolutionError => Error.Failure(description: "Host is not valid.", metadata: new()
                {
                    { "Exception", ex }
                }),
                HttpRequestError.ConnectionError => Error.Failure(
                    description: "Host is not reachable, Check if you're using the correct port.", metadata: new()
                    {
                        { "Exception", ex }
                    }),
                HttpRequestError.SecureConnectionError => Error.Failure(
                    description: "Couldn't establish a secure connection, Try using the http protocol instead.",
                    metadata: new()
                    {
                        { "Exception", ex }
                    }),

                _ => Error.Failure(description: "An error occurred while logging in.", metadata: new()
                {
                    { "Exception", ex }
                })
            };
        }
        catch (UriFormatException ex)
        {
            return Error.Failure(description: "Host is not valid.", metadata: new()
            {
                { "Exception", ex }
            });
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while logging in.", metadata: new()
            {
                { "Exception", ex }
            });
        }

        AddTorrentsRequest request = new(torrentUrls)
        {
            DownloadFolder = settings.Folder,
            Category = settings.Category,
            Tags = settings.Tags
        };

        try
        {
            await client.AddTorrentsAsync(request);
        }
        catch (QBittorrentClientRequestException ex)
        {

        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while adding torrents.", metadata: new()
            {
                { "Exception", ex }
            });
        }

        return new Success();
    }
}