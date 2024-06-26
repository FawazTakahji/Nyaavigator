using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Models;
using NLog;
using Notification = Nyaavigator.Models.Notification;

namespace Nyaavigator.Utilities;

internal static class Sneedex
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    internal static async Task<List<SneedexEntry>> GetIds()
    {
        List<SneedexEntry> entries = [];

        HttpClient httpClient = App.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("SneedexClient");

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync("api/public/nyaa");
            if (!response.IsSuccessStatusCode)
            {
                string message = $"A connection error occurred while fetching the list of Sneedex entries.\n\nStatus Code: {response.StatusCode}\nReason: {response.ReasonPhrase}";

                Logger.Error(message);
                new Notification("Sneedex Error",
                    "A connection error occurred while fetching the list of Sneedex entries.",
                    NotificationType.Error)
                    .Send();
            }
            else
            {
                string json = await response.Content.ReadAsStringAsync();
                entries = JsonSerializer.Deserialize<List<SneedexEntry>>(json) ?? entries;
            }
        }
        catch (Exception ex)
        {
            const string message = "An error occurred while fetching the list of Sneedex entries.";
            Logger.Error(ex, message);
            new Notification("Sneedex Error", message, NotificationType.Error)
                .Send();
        }

        return entries;
    }
}