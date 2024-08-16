using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using Avalonia.Controls.Primitives;

namespace Nyaavigator.Views;

public class DialogViewBase : UserControl
{
    protected TaskCompletionSource? _tcs;
    protected DialogHost? _host;
    protected IInputElement? _lastFocus;

    public virtual void Show()
    {
        _host = new DialogHost
        {
            Content = this
        };

        OverlayLayer? overlayLayer = OverlayLayer.GetOverlayLayer(App.TopLevel);
        if (overlayLayer == null)
            return;

        _lastFocus = App.TopLevel.FocusManager?.GetFocusedElement();
        overlayLayer.Children.Add(_host);
    }

    public virtual async Task ShowAsync()
    {
        _tcs = new TaskCompletionSource();
        Show();
        await _tcs.Task;
    }

    protected virtual void Hide()
    {
        if (_lastFocus != null)
        {
            _lastFocus.Focus();
            _lastFocus = null;
        }

        DataContext = null;

        OverlayLayer? overlayLayer = OverlayLayer.GetOverlayLayer(_host);
        overlayLayer?.Children.Remove(_host);
        _host.Content = null;

        _tcs?.TrySetResult();
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Handled)
        {
            base.OnKeyUp(e);
            return;
        }

        if (e.Key == Key.Escape)
        {
            Hide();
            e.Handled = true;
        }

        base.OnKeyUp(e);
    }
}