using Nyaavigator.Core.Storage;

namespace Nyaavigator.Android.Storage;

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

        _basePath = Application.Context.GetExternalFilesDir(null)?.AbsolutePath
                    ?? Application.Context.GetExternalFilesDir(null)?.AbsolutePath
                    ?? "/storage/emulated/0/Android/data/app.fawaztakahji.nyaavigator/files";
        return _basePath;
    }
}