using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using Nyaavigator.Builders;
using Nyaavigator.Converters.Json;
using Nyaavigator.Enums;
using Nyaavigator.Models;
using Nyaavigator.Utilities;
using Notification = Nyaavigator.Models.Notification;

namespace Nyaavigator.ViewModels;

public partial class LogsViewModel : ObservableObject
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public ObservableCollectionTarget CollectionTarget { get; } = (ObservableCollectionTarget)LogManager.Configuration.FindTargetByName("collection");

    [RelayCommand]
    private async Task Export()
    {
        if (CollectionTarget.Logs.Count <= 0)
        {
            new Notification("No Logs", type: NotificationType.Error).SendToLogsWindow();
            return;
        }

        LogEventInfo[] logs = CollectionTarget.Logs.ToArray();
        string json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { Converters = { new JsonLogEventInfoConverter() }, WriteIndented = true });

        IStorageFile? file = await Storage.JsonFilePickerAsync("Logs.json", App.LogsViewer);
        if (file == null)
        {
            new Notification("Export cancelled by user.", type: NotificationType.Error).SendToLogsWindow();
            return;
        }

        try
        {
            await File.WriteAllTextAsync(file.Path.LocalPath, json);
            if (File.Exists(file.Path.LocalPath))
            {
                new Notification("Logs Exported", "Click to open.", type: NotificationType.Success,
                    onClick: () => Storage.OpenFile(file.Path.LocalPath, App.LogsViewer)).SendToLogsWindow();
            }
        }
        catch (Exception ex)
        {
            string message = $"An error occurred while copying the file to \"{file.Path.LocalPath}\".";
            Logger.Error(ex, message);
            Dialog.Create(App.LogsViewer)
                .Type(DialogType.Error)
                .Content(message)
                .ShowAndForget();
        }
    }
}