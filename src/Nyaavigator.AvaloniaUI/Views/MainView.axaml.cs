using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Nyaavigator.Core.ViewModels;
using Ursa.Controls.OverlayShared;

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

        BackRequestedHandler.GlobalDialogBackRequested += OnDialogBackRequested;
        BackRequestedHandler.Subscribe(OnBackRequested);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        BackRequestedHandler.GlobalDialogBackRequested -= OnDialogBackRequested;
        BackRequestedHandler.Unsubscribe(OnBackRequested);
    }

    private void OnDialogBackRequested(object? sender, RoutedEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        OverlayFeedbackElement? overlay = GlobalHost.Children.OfType<OverlayFeedbackElement>().LastOrDefault();
        if (overlay is not null)
        {
            e.Handled = true;
            overlay.Close();
        }
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