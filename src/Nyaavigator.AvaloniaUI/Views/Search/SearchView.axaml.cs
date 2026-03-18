using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.ViewModels;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.OverlayShared;

namespace Nyaavigator.AvaloniaUI.Views.Search;

public partial class SearchView : UserControl
{
    public SearchView()
    {
        InitializeComponent();
        Bar.ShowSearchSettingsCommand = new RelayCommand(ShowSearchSettings);
    }

    private void ShowSearchSettings()
    {
        if (DataContext is not SearchViewModel viewModel)
        {
            return;
        }

        Drawer.ShowModal<SearchSettings, SearchViewModel>(viewModel, OverlayHost.HostId, new()
        {
            Position = Position.Bottom,
            Buttons = DialogButton.None,
            CanLightDismiss = true,
            IsCloseButtonVisible = true,
            Title = string.Empty,
            CanResize = false,
            StyleClass = "SearchSettings"
        });
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
        if (e.Handled)
        {
            return;
        }
        OverlayFeedbackElement? overlay = OverlayHost.Children.OfType<OverlayFeedbackElement>().LastOrDefault();
        if (overlay is not null)
        {
            e.Handled = true;
            overlay.Close();
        }
    }
}