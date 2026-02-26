using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nyaavigator.AvaloniaUI.Views;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.AvaloniaUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        MainView view = new MainView
        {
            DataContext = Ioc.Default.GetRequiredService<MainViewModel>()
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Content = view
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = view;
        }

        base.OnFrameworkInitializationCompleted();
    }
}