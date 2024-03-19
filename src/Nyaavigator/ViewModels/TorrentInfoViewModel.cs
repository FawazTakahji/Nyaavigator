using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Nyaavigator.Messages;
using Nyaavigator.Models;
using Nyaavigator.Utilities;

namespace Nyaavigator.ViewModels;

public partial class TorrentInfoViewModel : ObservableObject
{
    private readonly string? _link;
    [ObservableProperty]
    private TorrentInfo? _info;
    [ObservableProperty]
    private bool _noInfo;

    public TorrentInfoViewModel(string? link)
    {
        _link = link;
        GetInfoCommand.ExecuteAsync(null);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task GetInfo(CancellationToken token)
    {
        if (!string.IsNullOrEmpty(_link))
            Info = await Nyaa.GetTorrentInfo(_link, token);
        NoInfo = Info == null;
    }

    [RelayCommand]
    private async Task CopyHash()
    {
        await Clipboard.Copy(Info!.Hash!);
        WeakReferenceMessenger.Default.Send(new InfoViewMessage("Hash Copied."));
    }

// for previewer
#if DEBUG
    public TorrentInfoViewModel(TorrentInfo? info)
    {
        Info = info;
    }
#endif
}