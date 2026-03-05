using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Settings;

public partial class SettingsService : ObservableObject
{
    private const string FileName = "settings.json";
    private readonly IPersistentStorageService _storage;

    [ObservableProperty]
    private AppSettings _settings = new();

    public SettingsService(IPersistentStorageService storage)
    {
        _storage = storage;
    }

    public void Load()
    {
        string? json = _storage.Read(FileName);
        if (json is null)
        {
            return;
        }

        Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? throw new Exception("Deserialized settings are null.");
    }

    public void Save()
    {
        string json = JsonSerializer.Serialize(Settings);
        _storage.Write(FileName, json);
    }
}