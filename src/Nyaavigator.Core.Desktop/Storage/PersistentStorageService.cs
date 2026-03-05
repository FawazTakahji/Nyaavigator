using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Desktop.Storage;

public class PersistentStorageService : FilePersistentStorageService
{
    private static string? _basePath;
    protected override string BasePath => GetBasePath();

    public static string GetBasePath()
    {
        if (_basePath != null)
        {
            return _basePath;
        }

        string portablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portable");
        if (Directory.Exists(portablePath))
        {
            _basePath = portablePath;
            return _basePath;
        }

        // TODO: use different paths for linux and mac
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _basePath = Path.Combine(appDataPath, Constants.Title);
        return _basePath;
    }
}