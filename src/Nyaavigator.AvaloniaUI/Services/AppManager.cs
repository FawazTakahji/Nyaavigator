using System;
using Avalonia;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;
using Nyaavigator.Core.Toasts;

namespace Nyaavigator.AvaloniaUI.Services;

public class AppManager : IAppManager
{
    private readonly ILogger<AppManager> _logger;
    private readonly SettingsService _settingsService;
    private readonly IToastManager _toastManager;

    public AppManager(ILogger<AppManager>logger, SettingsService settingsService, IToastManager toastManager)
    {
        _logger = logger;
        _settingsService = settingsService;
        _toastManager = toastManager;
    }

    public void Initialize()
    {
        _logger.LogInformation("Initializing app");
        _logger.LogInformation("Loading settings");
        try
        {
            _settingsService.Load();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load settings");
            _toastManager.Show("Could not load app settings", ToastType.Error, showClose: true);
        }

        SetTheme(_settingsService.Settings.Theme);
    }

    public void SetTheme(Theme theme)
    {
        if (Application.Current is null)
        {
            _logger.LogWarning("Could not set theme, current application is null");
            _toastManager.Show("Could not set theme", ToastType.Warning, showClose: true);
            return;
        }

        Application.Current.RequestedThemeVariant = theme switch
        {
            Theme.Light => ThemeVariant.Light,
            Theme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}