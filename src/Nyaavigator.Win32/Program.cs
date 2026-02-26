using System;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.AvaloniaUI;
using Nyaavigator.Core.Desktop.Extensions;
using Nyaavigator.Core.Extensions;

namespace Nyaavigator.Win32;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        if (!Design.IsDesignMode)
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddCoreServices()
                    .AddDesktopServices()
                    .BuildServiceProvider());
        }

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}