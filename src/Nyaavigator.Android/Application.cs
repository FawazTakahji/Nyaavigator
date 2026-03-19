using System;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nyaavigator.Android.Extensions;
using Nyaavigator.AvaloniaUI;
using Nyaavigator.AvaloniaUI.Extensions;
using Nyaavigator.Core.Extensions;

namespace Nyaavigator.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
    protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddCoreServices()
                .AddUiServices()
                .AddAndroidServices()
                .BuildServiceProvider());

        ILogger<Application>? logger = Ioc.Default.GetService<ILogger<Application>>();
        if (logger is not null)
        {
            AndroidEnvironment.UnhandledExceptionRaiser += (_, e) =>
            {
                logger.LogCritical(e.Exception, "Unhandled Android exception");
                Ioc.Default.DisposeLogProviders();
            };

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                logger.LogCritical((Exception)e.ExceptionObject, "Unhandled domain exception");
                Ioc.Default.DisposeLogProviders();
            };

            TaskScheduler.UnobservedTaskException +=
                (_, e) => logger.LogError(e.Exception, "Unhandled task exception");
        }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}