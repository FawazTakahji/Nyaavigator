using System.Text.Json.Serialization;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Nyaavigator.Converters.Json;
using Nyaavigator.Enums;

namespace Nyaavigator.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty]
    private bool _systemAccent = true;
    [ObservableProperty] [property: JsonConverter(typeof(JsonColorConverter))]
    private Color _accentColor = Color.FromRgb(51, 122, 183);
    [ObservableProperty] [property: JsonConverter(typeof(JsonStringEnumConverter))]
    private Theme _theme;
    [ObservableProperty]
    private bool _hideTorrentsWithNoSeeders;
    [ObservableProperty]
    private bool _sneedexIntegration;
    public QBittorrentSettings QBittorrentSettings { get; set; } = new();

    // Stop the previewer from spamming GitHub api
    [ObservableProperty]
    private bool _checkUpdates =
#if DEBUG
        false;
#else
        true;
#endif
}