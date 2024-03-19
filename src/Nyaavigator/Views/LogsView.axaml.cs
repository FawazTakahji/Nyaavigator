using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Messages;
using Nyaavigator.ViewModels;
using NLog;
using System;
using Avalonia.Controls;

#if DEBUG
using Avalonia;
using Avalonia.Layout;
#endif

namespace Nyaavigator.Views;

public partial class LogsView : Window, IRecipient<NotificationLogsMessage>
{
    private WindowNotificationManager _notificationManager;

#if DEBUG
    private Window? _debugWindow;
#endif

    public LogsView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<LogsViewModel>();
        WeakReferenceMessenger.Default.Register(this);
        ((ObservableCollection<LogEventInfo>)DataGrid.ItemsSource).CollectionChanged += ScrollToBottom;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        ((ObservableCollection<LogEventInfo>)DataGrid.ItemsSource).CollectionChanged -= ScrollToBottom;

#if DEBUG
        _debugWindow?.Close();
#endif
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        _notificationManager = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.BottomRight
        };

#if DEBUG
        StackPanel panel = new()
        {
            Spacing = 5,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 2, 0, 5)
        };
        foreach (LogLevel logLevel in LogLevel.AllLoggingLevels)
        {
            RepeatButton button = new()
            {
                Content = logLevel.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            button.Click += (_, _) =>
            {
                LogManager.GetCurrentClassLogger()
                    .Log(logLevel, new Exception(logLevel.ToString()), logLevel.ToString());
            };
            panel.Children.Add(button);
        }

        _debugWindow = new Window
        {
            SizeToContent = SizeToContent.Height,
            Content = panel,
            Width = 200,
            CanResize = false,
            Title = string.Empty
        };
        _debugWindow.Show();
#endif
    }

    public void Receive(NotificationLogsMessage message)
    {
        _notificationManager.Show(message.Value);
    }

    private void ScrollToBottom(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is { Count: > 0 })
            DataGrid.ScrollIntoView(e.NewItems[^1], null);
    }
}