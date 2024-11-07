using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErrorOr;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Enums;
using Nyaavigator.Models;
using Nyaavigator.Utilities;
using Nyaavigator.Builders;
using Nyaavigator.Services;
using Nyaavigator.Views;
using NotificationType = Avalonia.Controls.Notifications.NotificationType;

namespace Nyaavigator.ViewModels;

public partial class WindowViewModel : ObservableObject
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public static IList<Filter> Filters { get; } = UI.GetFilters();
    public static IList<Category> Categories { get; } = UI.GetCategories();
    [ObservableProperty]
    private Filter _selectedFilter;
    [ObservableProperty]
    private Category _selectedCategory;
    [ObservableProperty]
    private bool? _isAllSelected = false;
    [ObservableProperty]
    private string _searchQuery = string.Empty;
    [ObservableProperty]
    private string _username = string.Empty;
    [ObservableProperty]
    private string _selectedSorting = "id";
    [ObservableProperty]
    private string _selectedOrder = "desc";

    public TorrentCollection Torrents { get; } = [];
    public SmartCollection<PageButton> Pages { get; } = [];
    [ObservableProperty]
    private string _resultsString = string.Empty;
    public SettingsService SettingsService { get; }
    private readonly SneedexService _sneedexService;

    public WindowViewModel()
    {
        SelectedFilter = Filters[0];
        SelectedCategory = Categories[0];

        SettingsService = App.ServiceProvider.GetRequiredService<SettingsService>();
        _sneedexService = App.ServiceProvider.GetRequiredService<SneedexService>();

        Torrents.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TorrentCollection.IsAnyTorrentDownloading))
                SearchCommand.NotifyCanExecuteChanged();
        };

        DownloadTorrentsCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DownloadTorrentsCommand.IsRunning))
            {
                CheckIsAllSelectedCommand.NotifyCanExecuteChanged();
                CheckSelectedCommand.NotifyCanExecuteChanged();
            }
        };

#if DEBUG
        Dispatcher.UIThread.InvokeAsync(CreateDebugItems);
#endif
    }

    [RelayCommand]
    private async Task ShowMoreInfo(string? link)
    {
        if (string.IsNullOrEmpty(link))
        {
            new Notification("Torrent link is invalid.", type:NotificationType.Error).Send();
            return;
        }

        TorrentInfoViewModel viewModel = new(link);
        await new TorrentInfoView(viewModel).ShowAsync();
    }

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanSearchExecute))]
    private async Task Search(string href, CancellationToken token)
    {
        IsAllSelected = false;
        Torrents.Clear();
        Pages.Clear();
        ResultsString = string.Empty;

        string searchString = string.Empty;
        if (string.IsNullOrEmpty(href))
        {
            if (!string.IsNullOrEmpty(Username))
                searchString += $"user/{Username}";

            searchString += $"?f={SelectedFilter.Id}&c={SelectedCategory.Id}&q={SearchQuery}&s={SelectedSorting}&o={SelectedOrder}";
        }
        else
        {
            searchString = href;
        }

        (List<Torrent> torrents, List<PageButton> pages, string resultsString) result;

        try
        {
            token.ThrowIfCancellationRequested();
            result = await Nyaa.Search(searchString, token);
        }
        catch (OperationCanceledException)
        {
            new Notification("Search cancelled by user.", type:NotificationType.Error).Send();
            return;
        }

        if (SettingsService.AppSettings.SneedexIntegration)
        {
            foreach (Torrent torrent in result.torrents)
            {
                torrent.IsBestRelease = await _sneedexService.IsBestRelease(torrent.Id);
            }
        }

        Torrents.AddRange(SettingsService.AppSettings.HideTorrentsWithNoSeeders
            ? result.torrents.Where(t => t.Seeders != "0")
            : result.torrents);
        Pages.AddRange(result.pages);
        ResultsString = result.resultsString;

        if (result.torrents.Count > 0 && Torrents.Count == 0)
        {
            new Notification("Results Hidden",
                "You have \"Hide torrents with no seeders\" enabled. To see all results, disable this setting.",
                NotificationType.Warning).Send();
        }
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task DownloadTorrents(CancellationToken token)
    {
        List<Torrent> selectedTorrents = Torrents.Where(t => t.IsSelected).ToList();
        selectedTorrents.ForEach(x => x.IsDownloading = true);

        IStorageFolder? folder = await Storage.TorrentFolderPickerAsync();
        if (folder == null)
        {
            new Notification("Download cancelled by user.", type: NotificationType.Error).Send();
            selectedTorrents.ForEach(x => x.IsDownloading = false);
            return;
        }

        try
        {
            foreach (Torrent torrent in selectedTorrents)
            {
                token.ThrowIfCancellationRequested();
                bool success = await torrent.Download(folder.Path.LocalPath, token);

                if (success)
                    torrent.IsSelected = false;
            }
        }
        catch (OperationCanceledException)
        {
            new Notification("Download cancelled by user.", type: NotificationType.Error).Send();
            selectedTorrents.ForEach(x => x.IsDownloading = false);
        }

        CheckSelected();
    }

    [RelayCommand]
    private async Task OpenMagnets()
    {
        Torrent[] selectedTorrents = Torrents.Where(t => t.IsSelected).ToArray();
        foreach (Torrent torrent in selectedTorrents)
        {
            if (string.IsNullOrEmpty(torrent.Magnet))
            {
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content($"Couldn't open the magnet link for \"{torrent.Name}\" because it's invalid.")
                    .ShowAndForget();
                continue;
            }

            Link.Open(torrent.Magnet);
            await Task.Delay(100);
        }
    }

    [RelayCommand]
    private async Task AddTorrentsToQBittorrent()
    {
        Torrent[] selectedTorrents = Torrents.Where(t => t.IsSelected).ToArray();
        List<string> validTorrents = [];

        foreach (Torrent torrent in selectedTorrents)
        {
            if (string.IsNullOrEmpty(torrent.Magnet))
            {
                Dialog.Create()
                    .Type(DialogType.Error)
                    .Content($"Couldn't add the magnet link for \"{torrent.Name}\" because it's invalid.")
                    .ShowAndForget();
                continue;
            }

            validTorrents.Add(torrent.Magnet);
        }

        if (validTorrents.Count == 0)
        {
            new Notification("No valid magnet links selected.", type: NotificationType.Error)
                .Send();
            return;
        }

        ErrorOr<Success> result = await Utilities.QBittorrent.AddTorrents(validTorrents, SettingsService.AppSettings.QBittorrentSettings);
        if (result.IsError)
        {
            await Dialog.Create()
                .Type(DialogType.Error)
                .Content($"{result.FirstError.Description}")
                .Show();

            if (result.FirstError.Metadata?["Exception"] is Exception ex)
            {
                Logger.Error(ex, "An error occurred while adding torrents to QBittorrent.");
            }
        }
        else
        {
            new Notification("Success", "Torrents added to QBittorrent.", NotificationType.Success)
                .Send();
        }
    }

    [RelayCommand]
    private async Task AddTorrentToQBittorrent(string? magnet)
    {
        if (string.IsNullOrEmpty(magnet))
        {
            await Dialog.Create()
                .Type(DialogType.Error)
                .Content("Magnet link is invalid.")
                .Show();
            return;
        }

        ErrorOr<Success> result = await Utilities.QBittorrent.AddTorrents([magnet], SettingsService.AppSettings.QBittorrentSettings);
        if (result.IsError)
        {
            await Dialog.Create()
                .Type(DialogType.Error)
                .Content($"{result.FirstError.Description}")
                .Show();

            if (result.FirstError.Metadata?["Exception"] is Exception ex)
                Logger.Error(ex, "An error occurred while adding torrent to QBittorrent.");
        }
        else
        {
            new Notification("Success", "Torrent added to QBittorrent.", NotificationType.Success)
                .Send();
        }
    }

    [RelayCommand]
    private void OpenSneedexEntry(int nyaaId)
    {
        string? entryId = _sneedexService.GetEntryId(nyaaId);

        if (string.IsNullOrEmpty(entryId))
        {
            new Notification("Couldn't retrieve the sneedex entry id.", type: NotificationType.Error)
                .Send();
            return;
        }

        Link.Open($"https://sneedex.moe/?{entryId}");
    }

    [RelayCommand(CanExecute = nameof(CanCheckIsAllSelectedExecute))]
    private void CheckIsAllSelected()
    {
        if (IsAllSelected == true)
        {
            foreach (Torrent torrent in Torrents)
                torrent.IsSelected = true;
        }
        else
        {
            foreach (Torrent torrent in Torrents)
                torrent.IsSelected = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCheckSelectedExecute))]
    private void CheckSelected()
    {
        if (Torrents.All(x => x.IsSelected))
            IsAllSelected = true;
        else if (Torrents.All(x => !x.IsSelected))
            IsAllSelected = false;
        else
            IsAllSelected = null;
    }

    private bool CanCheckIsAllSelectedExecute => Torrents.Count > 0 && !DownloadTorrentsCommand.IsRunning;
    private bool CanCheckSelectedExecute => !Torrents.IsAnyTorrentDownloading;
    private bool CanSearchExecute => !Torrents.IsAnyTorrentDownloading;

#if DEBUG
    private void CreateDebugItems()
    {
        List<Torrent> torrents =
        [
            new Torrent
            {
                Category = "Test",
                Name = "Test",
                Comments = 1,
                Date = DateTime.Now,
                Downloads = "1",
                Leechers = "1",
                Magnet = null,
                Seeders = "1",
                Size = "1 GiB",
                IsDownloading = true,
                IsSelected = true,
                IsBestRelease = true
            },
            new Torrent
            {
                Category = "1_1",
                Name = "Test",
                Comments = 0,
                Date = DateTime.Now - TimeSpan.FromDays(1),
                Downloads = "0",
                Leechers = "0",
                Magnet = null,
                Seeders = "0",
                Size = "1 GiB",
                IsDownloading = false,
                IsSelected = false,
                IsBestRelease = false
            }
        ];
        List<PageButton> pages =
        [
            new PageButton(
                new Material.Icons.Avalonia.MaterialIcon
                    { Kind = Material.Icons.MaterialIconKind.ChevronDoubleLeft }, string.Empty, true, false),
            new PageButton("1", string.Empty, true, false),
            new PageButton("2", string.Empty, true, false),
            new PageButton("...", string.Empty, false, false),
            new PageButton("29", string.Empty, true, false),
            new PageButton("30", string.Empty, true, false),
            new PageButton("31", string.Empty, true, false),
            new PageButton("32", string.Empty, true, false),
            new PageButton("33", string.Empty, true, false),
            new PageButton("34", string.Empty, true, false),
            new PageButton("35", string.Empty, true, false),
            new PageButton("36", string.Empty, true, false),
            new PageButton("37", string.Empty, true, false),
            new PageButton("38", string.Empty, true, false),
            new PageButton("39", string.Empty, true, false),
            new PageButton("40", string.Empty, true, false),
            new PageButton(
                new Material.Icons.Avalonia.MaterialIcon
                    { Kind = Material.Icons.MaterialIconKind.ChevronDoubleRight }, string.Empty, true, false)
        ];
        Torrents.Reset(torrents);
        Pages.Reset(pages);
        ResultsString = "Displaying results 976-1000 out of 1000 results.";
        IsAllSelected = null;
    }
#endif
}