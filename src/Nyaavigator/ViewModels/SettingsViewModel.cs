using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Extensions;
using Nyaavigator.Services;
using Nyaavigator.Utilities;
using Nyaavigator.Views;

namespace Nyaavigator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsService SettingsService { get; }
    public string CurrentVersion { get; } = $"v{typeof(SettingsViewModel).Assembly.GetName().Version!.GetMajorMinorBuild().ToString()}";

    [ObservableProperty]
    private string _qBittorrentTagText = string.Empty;

    public SettingsViewModel()
    {
        SettingsService = App.ServiceProvider.GetRequiredService<SettingsService>();
    }

    [RelayCommand]
    private static void OpenLogsViewer()
    {
        if (Avalonia.Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        if (desktop.Windows.Contains(App.LogsViewer))
        {
            App.LogsViewer.Activate();
            return;
        }

        App.LogsViewer = new LogsView();
        App.LogsViewer.Show();
    }

    [RelayCommand]
    private static void OpenDataFolder()
    {
        Storage.OpenFolder(App.BaseDirectory);
    }

    [RelayCommand]
    private static async Task CheckUpdate()
    {
        await Updates.CheckUpdate(true);
    }

    [RelayCommand]
    public void QBittorrentAddTag()
    {
        if (string.IsNullOrEmpty(QBittorrentTagText) || SettingsService.AppSettings.QBittorrentSettings.Tags.Any(x => x.Equals(QBittorrentTagText, StringComparison.OrdinalIgnoreCase)))
            return;

        SettingsService.AppSettings.QBittorrentSettings.Tags.Add(QBittorrentTagText);
        QBittorrentTagText = string.Empty;
    }

    [RelayCommand]
    private void QBittorrentRemoveTag(string tag)
    {
        SettingsService.AppSettings.QBittorrentSettings.Tags.Remove(tag);
    }

    [RelayCommand]
    private async Task QBittorrentSelectFolder()
    {
        if (App.TopLevel.StorageProvider is not { } storageProvider)
        {
            await Dialog.Create()
                .Type(DialogType.Error)
                .Content("Storage Provider is not available.")
                .Show();
            return;
        }

        IReadOnlyList<IStorageFolder> folder = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Torrents Folder",
            SuggestedStartLocation = await storageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads),
            AllowMultiple = false
        });
        if (folder.Count == 0)
            return;

        SettingsService.AppSettings.QBittorrentSettings.Folder = folder[0].Path.AbsolutePath;
    }
}