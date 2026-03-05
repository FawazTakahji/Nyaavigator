using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Desktop.Storage;

public class PersistentStorageService : IPersistentStorageService
{
    private static string? _basePath;

    public string? Load(string file)
    {
        string path = Path.Combine(GetBasePath(), file);

        if (!File.Exists(path))
        {
            return null;
        }

        return File.ReadAllText(path);
    }

    public void Save(string file, string data)
    {
        string basePath = GetBasePath();
        string path = Path.Combine(basePath, file);

        if (Path.GetDirectoryName(path) is not { } directory)
        {
            throw new Exception("Couldn't get directory name");
        }
        Directory.CreateDirectory(directory);

        File.WriteAllText(path, data);
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