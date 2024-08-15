using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using Nyaavigator.Enums;

namespace Nyaavigator.Utilities;

internal static class Settings
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static Models.Settings LoadSettings()
    {
        string path = Path.Combine(App.BaseDirectory, "Settings.json");
        Models.Settings settings = new();

        if (!File.Exists(path))
        {
            Logger.Info("The settings file doesn't exist, a new file will be created.");
            SaveSettings(settings);
            return settings;
        }

        try
        {
            string json = File.ReadAllText(path);
            settings = JsonSerializer.Deserialize<Models.Settings>(json) ?? settings;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred while loading the app settings, using default values.");
        }

        if (!Enum.IsDefined(typeof(Theme), settings.Theme))
            settings.Theme = 0;

        return settings;
    }

    public static void SaveSettings(Models.Settings settings)
    {
        string path = Path.Combine(App.BaseDirectory, "Settings.json");
        try
        {
            string json = JsonSerializer.Serialize(settings, Singletons.SerializerOptions);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred while saving the app settings.");
        }
    }

    public static void ApplySettings(Models.Settings settings)
    {
        var faTheme = (FluentAvaloniaTheme)Application.Current.Styles[0];

        faTheme.PreferUserAccentColor = settings.SystemAccent;
        faTheme.CustomAccentColor = settings.SystemAccent ? null : settings.AccentColor;

        switch (settings.Theme)
        {
            case Theme.System:
                Application.Current.RequestedThemeVariant = ThemeVariant.Default;
                faTheme.PreferSystemTheme = true;
                break;
            case Theme.Light:
                Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                faTheme.PreferSystemTheme = false;
                break;
            case Theme.Dark:
                Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                faTheme.PreferSystemTheme = false;
                break;
        }
    }
}