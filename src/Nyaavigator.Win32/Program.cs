using System;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nyaavigator.AvaloniaUI;
using Nyaavigator.AvaloniaUI.Extensions;
using Nyaavigator.Core.Desktop.Extensions;
using Nyaavigator.Core.Extensions;

namespace Nyaavigator.Win32;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddCoreServices()
                .AddDesktopServices()
                .AddUiServices()
                .BuildServiceProvider());

        var logger = Ioc.Default.GetService<ILogger<Program>>();
        if (logger is null)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            return;
        }

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            logger.LogCritical((Exception)e.ExceptionObject, "Unhandled domain exception");
            Ioc.Default.DisposeLogProviders();
        };

        TaskScheduler.UnobservedTaskException +=
            (_, e) => logger.LogError(e.Exception, "Unhandled task exception");

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled exception");
            Ioc.Default.DisposeLogProviders();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}