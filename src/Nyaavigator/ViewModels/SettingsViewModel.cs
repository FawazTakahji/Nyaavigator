using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.Core;
using Nyaavigator.Enums;
using Nyaavigator.Extensions;
using Nyaavigator.Utilities;
using Nyaavigator.Views;
using Settings = Nyaavigator.Utilities.Settings;

namespace Nyaavigator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private Models.Settings _appSettings = Settings.LoadSettings();
    [ObservableProperty]
    private Array _themeEnums = Enum.GetValues(typeof(Theme));
    [ObservableProperty]
    private string _currentVersion = $"v{typeof(SettingsViewModel).Assembly.GetName().Version!.GetMajorMinorBuild().ToString()}";

    public SettingsViewModel()
    {
        Settings.ApplySettings(AppSettings);

        AppSettings.PropertyChanged += (_, _) =>
        {
            Settings.ApplySettings(AppSettings);
            Settings.SaveSettings(AppSettings);
        };
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