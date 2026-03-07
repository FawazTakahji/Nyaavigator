using System.Globalization;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Utilities;

public static class Logs
{
    private static ILogger? _logger;
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
        TryGetLogger()?.LogInformation("Deleting old logs");
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
                        TryGetLogger()?.LogInformation("Deleting old log file {file}", file);
                        try
                        {
                            storageService.Delete(file);
                        }
                        catch (Exception e)
                        {
                            TryGetLogger()?.LogError(e, "Failed to delete old log file {file}", file);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            TryGetLogger()?.LogError(e, "Failed to delete old logs");
        }
    }

    private static ILogger? TryGetLogger()
    {
        _logger ??= Ioc.Default.GetService<ILoggerFactory>()?.CreateLogger(typeof(Logs));
        return _logger;
    }
}