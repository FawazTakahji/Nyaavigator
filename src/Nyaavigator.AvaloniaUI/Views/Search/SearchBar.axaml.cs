using Avalonia.Controls;
using Avalonia.Input;

namespace Nyaavigator.AvaloniaUI.Views.Search;

public partial class SearchBar : UserControl
{
    public SearchBar()
    {
        InitializeComponent();
    }

    private void SearchBar_OnGettingFocus(object? sender, FocusChangingEventArgs e)
    {
        if ((e.NavigationMethod == NavigationMethod.Pointer && (e.Source == MoreButton || e.Source == SearchButton))
            || e is { NavigationMethod: NavigationMethod.Unspecified, OldFocusedElement: MenuItem })
        {
            e.Handled = true;
            e.TryCancel();
        }
    }
}