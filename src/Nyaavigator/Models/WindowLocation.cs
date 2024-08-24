using System.Text.Json.Serialization;
using Avalonia.Controls;

namespace Nyaavigator.Models;

public class WindowLocation
{
    public int? Left { get; set; }
    public int? Top { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WindowState? WindowState { get; set; }
}