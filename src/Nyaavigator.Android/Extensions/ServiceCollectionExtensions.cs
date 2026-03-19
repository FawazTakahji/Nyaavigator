using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Android.Storage;
using Nyaavigator.Core.Storage;
using Nyaavigator.Core.Utilities;
using ZLogger;

namespace Nyaavigator.Android.Extensions;

public static class AndroidServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAndroidServices()
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