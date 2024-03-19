using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Nyaavigator.Enums;

namespace Nyaavigator.Utilities;

internal static class Storage
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static void OpenFile(string path, TopLevel? topLevel = null)
    {
        try
        {
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                }
            }.Start();
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while trying to open the file.\n\"{path}\"";
            Logger.Error(ex, message);
            Builders.Dialog.Create(topLevel)
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
        }
    }

    public static void OpenFolder(string path, TopLevel? topLevel = null)
    {
        try
        {
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                }
            }.Start();
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while trying to open the folder.\n\"{path}\"";
            Logger.Error(ex, message);
            Builders.Dialog.Create(topLevel)
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
        }
    }

    public static async Task<IStorageFile?> TorrentFilePickerAsync(string fileName, TopLevel? topLevel = null)
    {
        topLevel ??= App.TopLevel;

        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save As",
            SuggestedFileName = fileName,
            DefaultExtension = ".torrent",
            ShowOverwritePrompt = true,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads),
            FileTypeChoices = [FilePickerFileTypes.Torrent]
        });

        return file;
    }

    public static async Task<IStorageFolder?> TorrentFolderPickerAsync(TopLevel? topLevel = null)
    {
        topLevel ??= App.TopLevel;

        IReadOnlyList<IStorageFolder> folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folder",
            AllowMultiple = false,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads)
        });

        return folders.Count > 0 ? folders[0] : null;
    }

    public static async Task<IStorageFile?> JsonFilePickerAsync(string fileName, TopLevel? topLevel = null)
    {
        topLevel ??= App.TopLevel;

        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save As",
            SuggestedFileName = fileName,
            DefaultExtension = ".json",
            ShowOverwritePrompt = true,
            SuggestedStartLocation = await topLevel.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Documents),
            FileTypeChoices = [FilePickerFileTypes.Json]
        });

        return file;
    }

    private static class FilePickerFileTypes
    {
        public static FilePickerFileType Torrent { get; } = new("Torrent File")
        {
            Patterns = new[] { "*.torrent" },
            AppleUniformTypeIdentifiers = new[] { "org.bittorrent.torrent" },
            MimeTypes = new[] { "application/x-bittorrent" }
        };

        public static FilePickerFileType Json { get; } = new("Json File")
        {
            Patterns = new[] { "*.json" },
            AppleUniformTypeIdentifiers = new[] { "public.json" },
            MimeTypes = new[] { "application/json" }
        };
    }
}