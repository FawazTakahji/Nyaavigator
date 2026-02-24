using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.AvaloniaUI;
using Nyaavigator.Core.Extensions;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
    {
        if (!Design.IsDesignMode)
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddCoreServices()
                    .BuildServiceProvider());
        }

        return AppBuilder.Configure<App>();
    }
}