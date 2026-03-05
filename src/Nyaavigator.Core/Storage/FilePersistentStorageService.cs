namespace Nyaavigator.Core.Storage;

public abstract class FilePersistentStorageService : IPersistentStorageService
{
    protected abstract string BasePath { get; }

    public string? Read(string path)
    {
        string fullPath = Path.Combine(BasePath, path);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        return File.ReadAllText(fullPath);
    }

    public void Write(string path, string data)
    {
        string fullPath = Path.Combine(BasePath, path);

        if (Path.GetDirectoryName(fullPath) is not { } directory)
        {
            throw new Exception("Couldn't get directory name");
        }
        Directory.CreateDirectory(directory);

        File.WriteAllText(fullPath, data);
    }

    public void Delete(string path)
    {
        File.Delete(Path.Combine(BasePath, path));
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(Path.Combine(BasePath, path));
    }

    public string[] GetFiles(string path)
    {
        string basePath = BasePath;
        string fullPath = Path.Combine(basePath, path);

        string[] files = Directory.GetFiles(fullPath);
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace(basePath, "").TrimStart('/', '\\');
        }

        return files;
    }
}