using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Android.Extensions;
using Nyaavigator.AvaloniaUI;
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
                .AddAndroidServices()
                .BuildServiceProvider());
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}