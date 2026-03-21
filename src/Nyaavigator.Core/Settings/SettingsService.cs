using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Settings;

public partial class SettingsService : ObservableObject
{
    private const string FileName = "settings.json";
    private readonly ILogger<SettingsService> _logger;
    private readonly IPersistentStorageService _storage;

    [ObservableProperty]
    public partial string BaseUrl { get; set; } = AppSettings.DefaultBaseUrl;

    [ObservableProperty]
    public partial Theme Theme { get; set; } = AppSettings.DefaultTheme;

    public SettingsService(ILogger<SettingsService> logger, IPersistentStorageService storage)
    {
        _logger = logger;
        _storage = storage;
    }

    public void Load()
    {
        string? json = _storage.Read(FileName);
        if (json is null)
        {
            return;
        }
        AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);
        if (settings is null)
        {
            _logger.LogWarning("Failed to deserialize settings, using default settings.");
            return;
        }

        BaseUrl = settings.BaseUrl;
        Theme = settings.Theme;
    }

    public void Save()
    {
        AppSettings settings = new()
        {
            BaseUrl = BaseUrl,
            Theme = Theme
        };

        string json = JsonSerializer.Serialize(settings);
        _storage.Write(FileName, json);
    }
}