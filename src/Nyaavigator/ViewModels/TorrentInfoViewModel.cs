using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ErrorOr;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Messages;
using Nyaavigator.Models;
using Nyaavigator.Services;
using Nyaavigator.Utilities;

namespace Nyaavigator.ViewModels;

public partial class TorrentInfoViewModel : ObservableObject
{
    private readonly FeedService _feedService;
    private readonly string? _link;
    [ObservableProperty]
    private TorrentInfo? _info;
    [ObservableProperty]
    private bool _noInfo;
    [ObservableProperty]
    private bool _isUserAnonymous;
    [ObservableProperty]
    private bool _isUserFollowed;

    public TorrentInfoViewModel(string? link)
    {
        _link = link;
        GetInfoCommand.ExecuteAsync(null);
        _feedService = App.ServiceProvider.GetRequiredService<FeedService>();
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task GetInfo(CancellationToken token)
    {
        if (!string.IsNullOrEmpty(_link))
            Info = await Nyaa.GetTorrentInfo(_link, token);

        if (Info == null)
        {
            NoInfo = true;
        }
        else
        {
            NoInfo = false;

            if (!string.IsNullOrEmpty(Info.Submitter.Name))
            {
                IsUserAnonymous = false;
                IsUserFollowed = _feedService.IsUserFollowed(Info.Submitter.Name);
            }
            else
            {
                IsUserAnonymous = true;
            }
        }
    }

    [RelayCommand]
    private async Task FollowUser()
    {
        ErrorOr<Success> result = await _feedService.AddFeed(Info!.Submitter.Name!);
        if (result.IsError)
        {
            await Dialog.Create()
                .Type(DialogType.Error)
                .Content($"Failed to follow {Info!.Submitter.Name!}: {result.FirstError.Description}.")
                .Show();
        }

        IsUserFollowed = _feedService.IsUserFollowed(Info.Submitter.Name!);
    }

    [RelayCommand]
    private void UnfollowUser()
    {
        _feedService.RemoveFeed(Info!.Submitter.Name!);
        IsUserFollowed = _feedService.IsUserFollowed(Info.Submitter.Name!);
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