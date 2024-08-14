using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Nyaavigator.Messages;
using Nyaavigator.Models;
using Nyaavigator.ViewModels;

namespace Nyaavigator.Views;

public partial class WindowView : Window, IRecipient<NotificationMessage>
{
    private WindowNotificationManager _notificationManager;

    public WindowView()
    {
        InitializeComponent();
        DataContext = new WindowViewModel();

        WeakReferenceMessenger.Default.Register<NotificationMessage>(this);

        SettingsButton.Click += (_, _) =>
        {
            new SettingsView().Show();
        };
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        App.TopLevel = GetTopLevel(this);
        _notificationManager = new WindowNotificationManager(this)
        {
            Position = NotificationPosition.BottomRight
        };
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        App.LogsViewer?.Close();
        base.OnClosing(e);
    }

    public void Receive(NotificationMessage message)
    {
        _notificationManager.Show(message.Value);
    }

    private void SearchBarKeys(object? sender, KeyEventArgs e)
    {
        if (e.Handled || DataContext is not WindowViewModel viewModel)
            return;

        if (e.Key == Key.Enter && viewModel.SearchCommand.CanExecute(null))
            viewModel.SearchCommand.Execute(null);
        else if (e.Key == Key.Escape && sender is TextBox textBox)
            textBox.Text = string.Empty;
    }

    private void DataGrid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is not IDataContextProvider { DataContext: Torrent torrent } || this.DataContext is not WindowViewModel viewModel)
            return;

        if (viewModel.ShowMoreInfoCommand.CanExecute(null))
            viewModel.ShowMoreInfoCommand.Execute(torrent.Link);
    }

    private void DoubleTapBlock(object? sender, TappedEventArgs e)
    {
        e.Handled = true;
    }
}