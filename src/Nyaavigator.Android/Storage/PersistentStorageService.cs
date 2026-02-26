using System;
using System.IO;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Android.Storage;

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
        string? basePath = Application.Context.GetExternalFilesDir(null)?.AbsolutePath;
        if (string.IsNullOrEmpty(basePath))
        {
            throw new InvalidOperationException("Couldn't get application files directory.");
        }

        return basePath;
    }
}