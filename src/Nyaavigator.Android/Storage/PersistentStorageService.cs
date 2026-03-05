using System;
using System.IO;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Android.Storage;

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

        _basePath = Application.Context.GetExternalFilesDir(null)?.AbsolutePath
                    ?? Application.Context.GetExternalFilesDir(null)?.AbsolutePath
                    ?? "/storage/emulated/0/Android/data/app.fawaztakahji.nyaavigator/files";
        return _basePath;
    }
}