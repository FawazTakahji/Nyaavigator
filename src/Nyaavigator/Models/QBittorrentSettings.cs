using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Nyaavigator.Models;

public partial class QBittorrentSettings : ObservableObject
{
    [ObservableProperty]
    private string _host = "localhost";
    [ObservableProperty]
    private int _port = 8084;
    [ObservableProperty]
    private string _username = "admin";
    [ObservableProperty]
    private string _password = string.Empty;
    [ObservableProperty]
    private string _folder = string.Empty;
    [ObservableProperty]
    private string _category = string.Empty;
    public ObservableCollection<string> Tags { get; set; } = [];
}