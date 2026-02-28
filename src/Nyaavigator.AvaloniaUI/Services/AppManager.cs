using System;
using Avalonia.Styling;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;

namespace Nyaavigator.AvaloniaUI.Services;

public class AppManager : IAppManager
{
    private readonly SettingsService _settingsService;

    public AppManager(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void Initialize()
    {
        try
        {
            _settingsService.Load();
        }
        catch (Exception e)
        {
            // TODO: add logging
        }

        SetTheme(_settingsService.Settings.Theme);
    }

    public void SetTheme(Theme theme)
    {
        App.TopLevel?.RequestedThemeVariant = theme switch
        {
            Theme.System => ThemeVariant.Default,
            Theme.Light => ThemeVariant.Light,
            Theme.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}