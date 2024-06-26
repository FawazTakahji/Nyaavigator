using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.Core;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Extensions;
using Nyaavigator.Services;
using Nyaavigator.Utilities;
using Nyaavigator.Views;

namespace Nyaavigator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsService SettingsService { get; }
    public string CurrentVersion { get; } = $"v{typeof(SettingsViewModel).Assembly.GetName().Version!.GetMajorMinorBuild().ToString()}";

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
}