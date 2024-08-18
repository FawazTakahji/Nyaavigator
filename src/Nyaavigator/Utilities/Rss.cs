using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Models;
using ErrorOr;

namespace Nyaavigator.Utilities;

public static class Rss
{
    public static async Task<ErrorOr<List<RssRelease>>> GetUserReleases(string user)
    {
        IHttpClientFactory clientFactory = App.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient client = clientFactory.CreateClient("NyaaClient");
        Stream stream;

        try
        {
            HttpResponseMessage response = await client.GetAsync($"?page=rss&u={user}");
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.NotFound => Error.Failure(description: "The user doesn't exist."),
                    _ => Error.Failure(description: $"An error occurred while fetching the RSS feed.\n\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}")
                };
            }

            stream = await response.Content.ReadAsStreamAsync();
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while fetching the RSS feed.", metadata: new()
            {
                { "Exception", ex }
            });
        }

        XmlReader reader = XmlReader.Create(stream);
        SyndicationFeed feed = SyndicationFeed.Load(reader);

        return feed.Items
            .Select(item => new RssRelease(item.Title.Text, item.Id))
            .ToList();
    }
}