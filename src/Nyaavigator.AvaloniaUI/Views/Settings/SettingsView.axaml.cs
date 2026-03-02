using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.ViewModels;
using Ursa.Common;
using Ursa.Controls;

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

        Drawer.ShowModal<ThemeSelector, SettingsViewModel>(viewModel, SettingsDrawer.HostId, new()
        {
            Position = Position.Bottom,
            Buttons = DialogButton.None,
            CanLightDismiss = true,
            IsCloseButtonVisible = true,
            Title = "Select Theme",
            CanResize = false
        });
    }
}