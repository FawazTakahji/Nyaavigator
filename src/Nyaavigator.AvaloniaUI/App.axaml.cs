using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nyaavigator.AvaloniaUI.Views;
using Nyaavigator.AvaloniaUI.Windows;
using Nyaavigator.Core.Extensions;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Utilities;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.AvaloniaUI;

public partial class App : Application
{
    public static event EventHandler? MainViewLoaded;
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
        var logger = Ioc.Default.GetService<ILogger<App>>();
        if (logger is not null)
        {
            Dispatcher.UIThread.UnhandledException +=
                (_, e) => logger.LogError(e.Exception, "Unhandled UI exception");
        }

        MainView view = new MainView
        {
            DataContext = Ioc.Default.GetRequiredService<MainViewModel>()
        };
        view.Loaded += MainView_Loaded;

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

        Logs.DeleteOldLogs(Ioc.Default);
        base.OnFrameworkInitializationCompleted();
    }

    private void MainView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control control)
        {
            TopLevel = TopLevel.GetTopLevel(control);
            Ioc.Default.GetRequiredService<IAppManager>().Initialize();
            BackRequestedHandler.Initialize();
            MainViewLoaded?.Invoke(this, EventArgs.Empty);

            control.Loaded -= MainView_Loaded;
        }
    }
}