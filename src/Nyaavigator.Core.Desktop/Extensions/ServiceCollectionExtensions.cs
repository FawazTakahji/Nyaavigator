using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Desktop.Storage;
using Nyaavigator.Core.Storage;
using Nyaavigator.Core.Utilities;
using ZLogger;

namespace Nyaavigator.Core.Desktop.Extensions;

public static class DesktopServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDesktopServices()
        {
            return services.AddLogging()
                .AddSingleton<IPersistentStorageService, PersistentStorageService>();
        }

        private IServiceCollection AddLogging()
        {
            return services.AddLogging(logging =>
            {
#if RELEASE
                Microsoft.Extensions.Logging.LoggingBuilderExtensions.SetMinimumLevel(logging, Microsoft.Extensions.Logging.LogLevel.Information);
#endif
                logging.AddZLoggerFile(Path.Combine(PersistentStorageService.GetBasePath(), "logs", Logs.GetLogFileName(DateTimeOffset.Now)),
                    o => o.UseJsonFormatter());
            });
        }
    }
}