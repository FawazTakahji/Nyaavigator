namespace Nyaavigator.Core.Storage;

public interface IPersistentStorageService
{
    public string? Read(string path);
    public void Write(string path, string data);
    public void Delete(string path);
    public bool DirectoryExists(string path);
    public string[] GetFiles(string path);
}