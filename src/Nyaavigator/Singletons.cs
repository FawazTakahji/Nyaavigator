using System.Text.Json;

namespace Nyaavigator;

public static class Singletons
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };
}