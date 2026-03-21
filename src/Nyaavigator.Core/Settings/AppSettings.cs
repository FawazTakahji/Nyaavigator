using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nyaavigator.Core.Settings;

public class AppSettings : ObservableObject
{
    public string BaseUrl { get; init; } = DefaultBaseUrl;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Theme Theme { get; init; } = DefaultTheme;

    public int Version { get; set; } = 1;

    public const string DefaultBaseUrl = "https://nyaa.si";
    public const Theme DefaultTheme = Theme.System;
}