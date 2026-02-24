using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core;
using Nyaavigator.Core.Extensions;

namespace Nyaavigator.Android;

[Activity(
    Label = Constants.Title,
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        if (!Design.IsDesignMode)
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddCoreServices()
                    .BuildServiceProvider());
        }

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}