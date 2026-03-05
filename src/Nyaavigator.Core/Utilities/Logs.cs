using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Utilities;

public static class Logs
{
    private const string Format = "yyyy-MM-dd_HH.mm.sszzz";

    public static string GetLogFileName(DateTimeOffset dateTimeOffset)
    {
        return $"{dateTimeOffset.ToString(Format).Replace(':', 'Z')}.log";
    }

    private static DateTimeOffset? GetDateTimeOffset(string logFileName)
    {
        string date = Path.GetFileNameWithoutExtension(logFileName);
        if (DateTimeOffset.TryParseExact(date.Replace("Z", ":"), Format, null, DateTimeStyles.None, out DateTimeOffset dateTimeOffset))
        {
            return dateTimeOffset;
        }

        return null;
    }

    public static void DeleteOldLogs(IServiceProvider provider)
    {
        IPersistentStorageService storageService = provider.GetRequiredService<IPersistentStorageService>();

        try
        {
            if (!storageService.DirectoryExists("logs"))
            {
                return;
            }

            string[] files = storageService.GetFiles("logs");
            foreach (string file in files)
            {
                if (GetDateTimeOffset(file) is { } dateTimeOffset)
                {
                    if (DateTimeOffset.Now - dateTimeOffset > TimeSpan.FromDays(7))
                    {
                        try
                        {
                            storageService.Delete(file);
                        }
                        catch (Exception e)
                        {
                            // TODO: log
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            // TODO: log
        }
    }
}