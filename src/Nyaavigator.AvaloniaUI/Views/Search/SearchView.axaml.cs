using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.ViewModels;
using Ursa.Common;
using Ursa.Controls;

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

        Drawer.ShowModal<SearchSettings, SearchViewModel>(viewModel, Host.HostId, new()
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
}