using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Desktop.Storage;

public class PersistentStorageService : IPersistentStorageService
{
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
        Directory.CreateDirectory(basePath);
        File.WriteAllText(path, data);
    }

    private string GetBasePath()
    {
        string portablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "portable");
        if (Directory.Exists(portablePath))
        {
            return portablePath;
        }

        // TODO: use different paths for linux and mac
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(basePath, Constants.Title);
    }
}