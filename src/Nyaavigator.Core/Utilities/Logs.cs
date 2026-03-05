namespace Nyaavigator.Core.Utilities;

public static class Logs
{
    private const string Format = "yyyy-MM-dd_HH.mm.sszzz";

    public static string GetLogFileName(DateTimeOffset dateTimeOffset)
    {
        return $"{dateTimeOffset.ToString(Format).Replace(':', 'Z')}.log";
    }
}