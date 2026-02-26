namespace Nyaavigator.Core.Storage;

public interface IPersistentStorageService
{
    public string? Load(string file);
    public void Save(string file, string data);
}