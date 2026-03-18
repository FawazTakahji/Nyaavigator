using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Desktop.Storage;
using Nyaavigator.Core.Storage;
using Nyaavigator.Core.Utilities;
using ZLogger;

namespace Nyaavigator.Core.Desktop.Extensions;

public static class DesktopServices
{
    public static IServiceCollection AddDesktopServices(this IServiceCollection services)
    {
        return services.AddSingleton<IPersistentStorageService, PersistentStorageService>()
            .AddLogging(logging =>
            {
#if RELEASE
                Microsoft.Extensions.Logging.LoggingBuilderExtensions.SetMinimumLevel(logging, Microsoft.Extensions.Logging.LogLevel.Information);
#endif
                logging.AddZLoggerFile(Path.Combine(PersistentStorageService.GetBasePath(), "logs", Logs.GetLogFileName(DateTimeOffset.Now)),
                    o => o.UseJsonFormatter());
            });
    }
}