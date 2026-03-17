using Avalonia.Controls;
using Avalonia.Interactivity;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.AvaloniaUI.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        BackRequestedHandler.Subscribe(OnBackRequested);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        BackRequestedHandler.Unsubscribe(OnBackRequested);
    }

    private void OnBackRequested(object? sender, RoutedEventArgs e)
    {
        if (e.Handled || DataContext is not MainViewModel viewModel || !viewModel.NavigationService.CanGoBack)
        {
            return;
        }

        e.Handled = true;
        viewModel.NavigationService.Pop();
    }
}