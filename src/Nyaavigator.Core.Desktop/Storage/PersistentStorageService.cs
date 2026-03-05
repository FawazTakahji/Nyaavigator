using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Desktop.Storage;

public class PersistentStorageService : IPersistentStorageService
{
    private static string? _basePath;

    public string? Read(string path)
    {
        string fullPath = Path.Combine(GetBasePath(), path);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        return File.ReadAllText(fullPath);
    }

    public void Write(string path, string data)
    {
        string fullPath = Path.Combine(GetBasePath(), path);

        if (Path.GetDirectoryName(fullPath) is not { } directory)
        {
            throw new Exception("Couldn't get directory name");
        }
        Directory.CreateDirectory(directory);

        File.WriteAllText(fullPath, data);
    }

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