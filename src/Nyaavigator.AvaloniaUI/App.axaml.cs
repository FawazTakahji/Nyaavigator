using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nyaavigator.AvaloniaUI.Views;
using Nyaavigator.AvaloniaUI.Windows;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.AvaloniaUI;

public partial class App : Application
{
    public static TopLevel? TopLevel { get; private set; }

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
            TopLevel = desktop.MainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = view;
            TopLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
        }

        Ioc.Default.GetRequiredService<IAppManager>().Initialize();

        base.OnFrameworkInitializationCompleted();
    }
}