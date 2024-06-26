using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Extensions;
using Nyaavigator.Utilities;

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
    public int Id => string.IsNullOrEmpty(Href) ? 0 : Href.TryGetEndingInt();

    [ObservableProperty]
    private bool _isDownloading;
    [ObservableProperty]
    private bool _isSelected;
    public bool IsBestRelease { get; set; }

    [RelayCommand(IncludeCancelCommand = true)]
    public async Task OpenDownloadDialog(CancellationToken token)
    {
        IStorageFolder? folder = await Storage.TorrentFolderPickerAsync();
        if (folder == null)
        {
            new Notification("Download cancelled by user.", type: NotificationType.Error).Send();
            return;
        }

        try
        {
            await Download(folder.Path.LocalPath, token);
        }
        catch (OperationCanceledException)
        {
            new Notification("Download cancelled by user.", type: NotificationType.Error).Send();
            IsDownloading = false;
        }
    }

    public async Task<bool> Download(string downloadFolder, CancellationToken token)
    {
        if (DownloadHref == null)
        {
            new Notification("Download Failed", $"The download link for \"{Name}\" is invalid.", NotificationType.Error).Send();
            return false;
        }

        IsDownloading = true;

        (Stream? Stream, string? Name) torrentFile = await Nyaa.GetTorrentFile(DownloadHref, token);

        if (torrentFile.Stream == null)
        {
            IsDownloading = false;
            return false;
        }

        string fileName = torrentFile.Name ?? Path.GetFileName(DownloadHref);
        string baseName = Path.GetFileNameWithoutExtension(fileName);
        string filePath = Path.Combine(downloadFolder, fileName);
        for (int i = 1; File.Exists(filePath); i++)
        {
            token.ThrowIfCancellationRequested();

            fileName = $"{baseName} ({i}).torrent";
            filePath = Path.Combine(downloadFolder, fileName);
        }

        try
        {
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await torrentFile.Stream.CopyToAsync(fileStream);
            if (File.Exists(filePath))
                new Notification("Download Complete", $"{fileName}\n\nClick to open.", NotificationType.Success,
                    onClick: () => Storage.OpenFile(filePath)).Send();

            IsDownloading = false;
            return true;
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while copying the file to \"{filePath}\".";
            LogManager.GetCurrentClassLogger().Error(ex, message);
            Dialog.Create()
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
        }

        IsDownloading = false;
        return false;
    }
}