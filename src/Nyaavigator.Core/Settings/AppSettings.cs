using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nyaavigator.Core.Settings;

public partial class AppSettings : ObservableObject
{
    [ObservableProperty]
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    private Theme _theme;

    public int Version { get; set; } = 1;
}