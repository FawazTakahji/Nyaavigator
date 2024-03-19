using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Extensions;

namespace Nyaavigator.Utilities;

internal static class Updates
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static async Task CheckUpdate(bool noUpdateDialog = false)
    {
        Version currentVersion = typeof(Updates).Assembly.GetName().Version!.GetMajorMinorBuild();
        Version? githubVersion = await GetGithubVersion();

        if (githubVersion == null)
            return;

        if (githubVersion.CompareTo(currentVersion) > 0)
        {
            TaskDialogStandardResult result = await Dialog.Create()
                .Type(DialogType.Info)
                .Header("Update Available")
                .Content($"A newer version of the app is available (v{githubVersion})\nWould you like to update?")
                .Buttons([TaskDialogButton.YesButton, TaskDialogButton.NoButton])
                .Show();

            if (result == TaskDialogStandardResult.Yes)
                Link.Open("https://github.com/FawazTakhji/Nyaavigator/releases/latest");
        }
        else if (noUpdateDialog)
        {
            await Dialog.Create()
                .Type(DialogType.Info)
                .Header("No Updates")
                .Content("You are running the latest version.")
                .Show();
        }
    }

    private static async Task<Version?> GetGithubVersion()
    {
        HttpClient client = App.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("GithubClient");
        Version? latestVersion = null;

        HttpResponseMessage response;
        string responseContent;

        try
        {
            response = await client.GetAsync("repos/FawazTakhji/Nyaavigator/releases/latest");
            responseContent = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            string message = "An error occured while retrieving the latest app version.";
            Logger.Error(ex, message);
            Dialog.Create()
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
            return null;
        }

        if (response.IsSuccessStatusCode)
        {
            JsonObject? responseObj = JsonNode.Parse(responseContent)?.AsObject();

            if (responseObj != null && responseObj.TryGetPropertyValue("tag_name", out JsonNode? node))
            {
                if (Version.TryParse(node!.ToString(), out Version? version))
                    latestVersion = version;
            }
            else
            {
                string message = "Failed to get a valid version number from the GitHub api.";
                Logger.Error(message);
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content(message)
                    .ShowAndForget();
            }
        }
        else
        {
            string message = $"Failed to get a valid version number from the GitHub api.\nStatus Code: \"{response.StatusCode}\" - Reason Phrase: \"{response.ReasonPhrase}\".";
            Logger.Error(message);
            Dialog.Create()
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
        }

        return latestVersion;
    }
}