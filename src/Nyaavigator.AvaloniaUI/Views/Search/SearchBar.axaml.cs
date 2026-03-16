using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Nyaavigator.AvaloniaUI.Views.Search;

public partial class SearchBar : UserControl
{
    public static readonly StyledProperty<ICommand> ShowSearchSettingsCommandProperty = AvaloniaProperty.Register<SearchBar, ICommand>(
        nameof(ShowSearchSettingsCommand));

    public ICommand ShowSearchSettingsCommand
    {
        get => GetValue(ShowSearchSettingsCommandProperty);
        set => SetValue(ShowSearchSettingsCommandProperty, value);
    }

    public SearchBar()
    {
        InitializeComponent();
    }

    private void SearchBar_OnGettingFocus(object? sender, FocusChangingEventArgs e)
    {
        if ((e.NavigationMethod == NavigationMethod.Pointer && (e.Source == MoreButton || e.Source == SearchButton || e.Source == FilterButton))
            || e is { NavigationMethod: NavigationMethod.Unspecified, OldFocusedElement: MenuItem })
        {
            e.Handled = true;
            e.TryCancel();
        }
    }
}