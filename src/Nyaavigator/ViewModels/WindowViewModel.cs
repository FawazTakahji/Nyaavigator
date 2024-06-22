using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public SmartCollection<Torrent> Torrents { get; } = [];
    public SmartCollection<PageButton> Pages { get; } = [];
    [ObservableProperty]
    private string _resultsString = string.Empty;

    [ObservableProperty]
    private DataGridCollectionView _torrentsView;
    public SettingsService SettingsService { get; }

    public WindowViewModel()
    {
        SelectedFilter = Filters[0];
        SelectedCategory = Categories[0];

        // TODO: settings should be its own service
        SettingsService = App.ServiceProvider.GetRequiredService<SettingsService>();

        DownloadTorrentsCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DownloadTorrentsCommand.IsRunning))
                CheckIsAllSelectedCommand.NotifyCanExecuteChanged();
        };

        TorrentsView = new DataGridCollectionView(Torrents, true, true)
        {
            Filter = item => !SettingsService.AppSettings.HideTorrentsWithNoSeeders || ((Torrent)item).Seeders != "0"
        };

        TorrentsView.CollectionChanged += (_, _) =>
        {
            CheckIsAllSelectedCommand.NotifyCanExecuteChanged();
        };

        SettingsService.AppSettings.PropertyChanged += OnHideTorrentsChanged;

#if DEBUG
        Dispatcher.UIThread.InvokeAsync(CreateDebugItems);
#endif
    }

    private void OnHideTorrentsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SettingsService.AppSettings.HideTorrentsWithNoSeeders))
            return;

        TorrentsView.Refresh();

        switch (TorrentsView.IsEmpty)
        {
            case true when IsAllSelected is null or true:
            {
                IsAllSelected = false;
                foreach (Torrent torrent in Torrents)
                {
                    torrent.IsSelected = false;
                }

                break;
            }
            case false when IsAllSelected is null or true:
            {
                foreach (Torrent torrent in Torrents)
                {
                    if (!TorrentsView.Contains(torrent))
                        torrent.IsSelected = false;
                }
                CheckSelected();

                break;
            }
        }
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
        await new TorrentInfoView(viewModel).Show();
    }

    [RelayCommand(IncludeCancelCommand = true)]
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
            searchString = href;

        try
        {
            token.ThrowIfCancellationRequested();
            (List<Torrent> torrents, List<PageButton> pages, string resultsString) = await Nyaa.Search(searchString, token);
            token.ThrowIfCancellationRequested();
            Torrents.AddRange(torrents);
            Pages.AddRange(pages);
            ResultsString = resultsString;
        }
        catch (OperationCanceledException)
        {
            new Notification("Search cancelled by user.", type:NotificationType.Error).Send();
        }

        if (Torrents.Count > 0 && TorrentsView.IsEmpty)
        {
            new Notification("Results Hidden",
                "You have \"Hide torrents with no seeders\" enabled. To see all results, disable this setting.",
                NotificationType.Warning).Send();
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    private static async Task DownloadTorrent(Torrent torrent)
    {
        if (torrent.DownloadHref == null)
        {
            new Notification("Download Failed", $"The download link for \"{torrent.Name}\" is invalid.", NotificationType.Error).Send();
            return;
        }

        torrent.IsDownloading = true;

        (Stream? Stream, string? Name) torrentFile = await Nyaa.GetTorrentFile(torrent.DownloadHref);
        if (torrentFile.Stream == null)
        {
            torrent.IsDownloading = false;
            return;
        }

        string fileName = torrentFile.Name ?? Path.GetFileName(torrent.DownloadHref);

        IStorageFile? file = await Storage.TorrentFilePickerAsync(fileName);
        if (file == null)
        {
            new Notification("Download cancelled by user.", type: NotificationType.Error).Send();
            torrent.IsDownloading = false;
            return;
        }

        try
        {
            await using var fileStream = new FileStream(file.Path.LocalPath, FileMode.Create);
            await torrentFile.Stream.CopyToAsync(fileStream);
            if (File.Exists(file.Path.LocalPath))
                new Notification("Download Complete", $"{fileName}\n\nClick to open.", NotificationType.Success,
                    onClick: () => Storage.OpenFile(file.Path.LocalPath)).Send();
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while copying the file to \"{file.Path.LocalPath}\".";
            Logger.Error(ex, message);
            Dialog.Create()
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
        }

        torrent.IsDownloading = false;
    }

    [RelayCommand]
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
                if (torrent.DownloadHref == null)
                {
                    new Notification("Download Failed", $"The download link for \"{torrent.Name}\" is invalid.", NotificationType.Error).Send();
                    continue;
                }

                (Stream? Stream, string? Name) torrentFile = await Nyaa.GetTorrentFile(torrent.DownloadHref, token);
                if (torrentFile.Stream == null)
                {
                    torrent.IsDownloading = false;
                    await Task.Delay(1000, token);
                    continue;
                }

                string fileName = torrentFile.Name ?? Path.GetFileName(torrent.DownloadHref);
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                string filePath = Path.Combine(folder.Path.LocalPath, fileName);
                for (int i = 1; File.Exists(filePath); i++)
                {
                    token.ThrowIfCancellationRequested();

                    fileName = $"{baseName} ({i}).torrent";
                    filePath = Path.Combine(folder.Path.LocalPath, fileName);
                }

                try
                {
                    await using var fileStream = new FileStream(filePath, FileMode.CreateNew);
                    await torrentFile.Stream.CopyToAsync(fileStream);
                    if (File.Exists(filePath))
                        new Notification("Download Complete", $"{fileName}\n\nClick to open.", NotificationType.Success,
                            onClick: () => Storage.OpenFile(filePath)).Send();
                }
                catch (Exception ex)
                {
                    string message = $"An error occurred while copying the file to \"{filePath}\".";
                    Logger.Error(ex, message);
                    Dialog.Create()
                        .Type(DialogType.Error)
                        .Content(message)
                        .ShowAndForget();
                }

                torrent.IsDownloading = false;
                await Task.Delay(1000, token);
                token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException)
        {
            new Notification("Download cancelled by user.", type: NotificationType.Error).Send();
        }

        selectedTorrents.ForEach(x => x.IsDownloading = false);
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

    [RelayCommand(CanExecute = nameof(CanCheckIsAllSelectedExecute))]
    private void CheckIsAllSelected()
    {
        if (IsAllSelected == true)
        {
            foreach (Torrent torrent in TorrentsView)
                torrent.IsSelected = true;
        }
        else
        {
            foreach (Torrent torrent in TorrentsView)
                torrent.IsSelected = false;
        }
    }

    [RelayCommand]
    private void CheckSelected()
    {
        if (Torrents.All(x => x.IsSelected))
            IsAllSelected = true;
        else if (Torrents.All(x => !x.IsSelected))
            IsAllSelected = false;
        else
            IsAllSelected = null;
    }

    private bool CanCheckIsAllSelectedExecute()
    {
        return !TorrentsView.IsEmpty && !DownloadTorrentsCommand.IsRunning;
    }

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
                IsSelected = true
            },
            new Torrent
            {
                Category = "Test",
                Name = "Test",
                Comments = 0,
                Date = DateTime.Now - TimeSpan.FromDays(1),
                Downloads = "0",
                Leechers = "0",
                Magnet = null,
                Seeders = "0",
                Size = "1 GiB",
                IsDownloading = false,
                IsSelected = false
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