namespace Nyaavigator.Core.Storage;

public interface IPersistentStorageService
{
    public string? Read(string path);
    public void Write(string path, string data);
}