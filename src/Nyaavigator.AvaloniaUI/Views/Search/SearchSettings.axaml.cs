using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.ViewModels;
using Ursa.Controls.OverlayShared;

namespace Nyaavigator.AvaloniaUI.Views.Search;

public partial class SearchSettings : UserControl
{
    public static readonly StyledProperty<ICommand> CloseCommandProperty = AvaloniaProperty.Register<SearchSettings, ICommand>(
        nameof(CloseCommand));

    public ICommand CloseCommand
    {
        get => GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public SearchSettings()
    {
        InitializeComponent();
        CloseCommand = new RelayCommand<OverlayFeedbackElement>(Close);
    }

    private static void Close(OverlayFeedbackElement? owner)
    {
        owner?.Close();
    }

    private void ToggleButton_OnIsCheckedCategoryChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton { IsChecked: false } toggleButton || DataContext is not SearchViewModel viewModel)
        {
            return;
        }

        if (toggleButton.DataContext == viewModel.SelectedCategory.Parent
            || viewModel.SelectedCategory.Parent is null && toggleButton.DataContext is EmptyObject)
        {
            toggleButton.IsChecked = true;
        }
    }
}