using CommunityToolkit.Mvvm.ComponentModel;

namespace Nyaavigator.Models;

[INotifyPropertyChanged]
public partial class Torrent : TorrentBase
{
    private string? _downloadHref;
    public string? DownloadHref
    {
        get => _downloadHref;
        set => _downloadHref = value?.TrimStart('/');
    }
    public int Comments { get; set; }
    public string? Href { get; set; }
    public string? Magnet { get; set; }
    public string? Link => string.IsNullOrEmpty(Href) ? null : $"https://nyaa.si/{Href.TrimStart('/')}";

    [ObservableProperty]
    private bool _isDownloading;
    [ObservableProperty]
    private bool _isSelected;
}