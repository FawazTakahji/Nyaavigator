using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.ViewModels;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.OverlayShared;

namespace Nyaavigator.AvaloniaUI.Views.Settings;

public partial class SettingsView : UserControl
{
    public ICommand OpenThemeSelectorCommand { get; set; }

    public SettingsView()
    {
        InitializeComponent();

        OpenThemeSelectorCommand = new RelayCommand(OpenThemeSelector);
    }

    private void OpenThemeSelector()
    {
        if (DataContext is not SettingsViewModel viewModel)
        {
            return;
        }

        Drawer.ShowModal<ThemeSelector, SettingsViewModel>(viewModel, OverlayHost.HostId, new()
        {
            Position = Position.Bottom,
            Buttons = DialogButton.None,
            CanLightDismiss = true,
            IsCloseButtonVisible = true,
            Title = "Select Theme",
            CanResize = false
        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        BackRequestedHandler.Subscribe(OnBackRequested);
        ThemeSetting.Focus();
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