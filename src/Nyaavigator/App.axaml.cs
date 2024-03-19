using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using Nyaavigator.Builders;
using Nyaavigator.Extensions;
using Nyaavigator.Models;
using Nyaavigator.Utilities;
using Nyaavigator.ViewModels;
using Nyaavigator.Views;

namespace Nyaavigator;

public partial class App : Application
{
    public static string BaseDirectory { get; private set; }
    public static TopLevel TopLevel { get; set; }
    public static ServiceProvider ServiceProvider { get; private set; }
    public static Window? LogsViewer { get; set; }

#if (WINDOWS && RELEASE)
    private static readonly System.Threading.Mutex LocalMutex = new (true, "FawazT:Nyaavigator");
    private static readonly System.Threading.Mutex GlobalMutex = new (true, @"Global\FawazT:Nyaavigator");
#endif

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ConfigureNLog();
        BaseDirectory = GetBaseDirectory();
        ServiceProvider = CreateServiceProvider();
        ConfigureExceptionHandling();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new WindowView();
        }

        base.OnFrameworkInitializationCompleted();

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var viewmodel = ServiceProvider.GetRequiredService<SettingsViewModel>();
            if (viewmodel.AppSettings.CheckUpdates)
                await Utilities.Updates.CheckUpdate();
        });
    }

    private static void ConfigureNLog()
    {
        LoggingConfiguration config = new();
        ObservableCollectionTarget collectionTarget = new();
        config.AddTarget("collection", collectionTarget);
        config.AddRule(new LoggingRule("*", LogLevel.Trace, collectionTarget));
        LogManager.Configuration = config;
    }

    private static string GetBaseDirectory()
    {
#if PORTABLE
        string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
#elif WINDOWS
        string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".Nyaavigator");
#else
        string baseDir = "/etc/nyaavigator";
#endif
        try
        {
            Directory.CreateDirectory(baseDir);
        }
        catch (Exception ex)
        {
            LogManager.GetCurrentClassLogger().Error(ex, "Failed to create the app data directory.");
        }

        return baseDir;
    }

    private static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection serviceCollection = [];

        serviceCollection.AddSingleton<WindowViewModel>();
        serviceCollection.AddSingleton<SettingsViewModel>();
        serviceCollection.AddTransient<LogsViewModel>();

        Version version = typeof(App).Assembly.GetName().Version.GetMajorMinorBuild();
        string userAgent = $"Nyaavigator/{version} ({RuntimeInformation.OSDescription}; {RuntimeInformation.RuntimeIdentifier})";

        serviceCollection.AddHttpClient("NyaaClient", client =>
        {
            client.BaseAddress = new Uri("https://nyaa.si/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html; charset=UTF-8");
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        serviceCollection.AddHttpClient("GithubClient", client =>
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        });

        return serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureExceptionHandling()
    {
        AsyncAwaitBestPractices.SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                TaskDialogCommand copyExceptionButton = new()
                {
                    Text = "Copy Error",
                    IconSource = new SymbolIconSource { Symbol = Symbol.Copy },
                    ClosesOnInvoked = true
                };
                copyExceptionButton.Click += async (_, _) =>
                {
                    await Clipboard.Copy(ex.ToString());
                    new Notification("Error Copied", expiration: TimeSpan.FromSeconds(1),
                        onClose: () => { ((Window)TopLevel).Close(); }).Send();
                };

                Dialog.Create()
                    .Type(Enums.DialogType.Error)
                    .Content($"An error occured and the app is about to close.\n\nError:\n{ex.Message}")
                    .Commands([copyExceptionButton])
                    .NewWindow()
                    .ShowAndForget();
            });
        });
    }
}