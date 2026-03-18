using System;
using System.IO;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Android.Extensions;
using Nyaavigator.Android.Storage;
using Nyaavigator.AvaloniaUI;
using Nyaavigator.AvaloniaUI.Extensions;
using Nyaavigator.Core.Extensions;
using Nyaavigator.Core.Utilities;
using ZLogger;

namespace Nyaavigator.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
    protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddCoreServices()
                .AddAndroidServices()
                .AddUiServices()
                .AddLogging(logging =>
                {
#if RELEASE
                    Microsoft.Extensions.Logging.LoggingBuilderExtensions.SetMinimumLevel(logging, Microsoft.Extensions.Logging.LogLevel.Information);
#endif
                    logging.AddZLoggerFile(Path.Combine(PersistentStorageService.GetBasePath(), "logs", Logs.GetLogFileName(DateTimeOffset.Now)),
                        o => o.UseJsonFormatter());
                })
                .BuildServiceProvider());
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}