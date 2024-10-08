﻿using Nyaavigator.Models;
using SettingsUtils = Nyaavigator.Utilities.Settings;

namespace Nyaavigator.Services;

public class SettingsService
{
    public Settings AppSettings { get; }

    public SettingsService()
    {
        AppSettings = SettingsUtils.LoadSettings();
        SettingsUtils.ApplySettings(AppSettings);

        AppSettings.PropertyChanged += (_, _) =>
        {
            SettingsUtils.ApplySettings(AppSettings);
            SettingsUtils.SaveSettings(AppSettings);
        };
        AppSettings.QBittorrentSettings.PropertyChanged += (_, _) =>
        {
            SettingsUtils.SaveSettings(AppSettings);
        };
        AppSettings.QBittorrentSettings.Tags.CollectionChanged += (_, _) =>
        {
            SettingsUtils.SaveSettings(AppSettings);
        };
    }
}