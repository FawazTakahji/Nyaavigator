using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using Material.Icons;
using Nyaavigator.ViewModels;

namespace Nyaavigator.Views;

public partial class SettingsView : UserControl
{
    private DialogHost _host;
    private IInputElement? _lastFocus;

    public SettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();

        CloseButton.Click += (_, _) => Hide();

        AccentExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.PaletteSwatchOutline)) };
        SneedexExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.StarShooting)) };
        LogsExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.TextBoxOutline)) };
        FolderExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Folder)) };
        UpdatesExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Sync)) };
        RepoExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Github)) };
    }

    public void Show()
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

        this.Loaded += (_, _) => CloseButton.Focus();
    }

    private void Hide()
    {
        if (_lastFocus != null)
        {
            _lastFocus.Focus();
            _lastFocus = null;
        }

        DataContext = null;

        OverlayLayer? overlayLayer = OverlayLayer.GetOverlayLayer(_host);
        if (overlayLayer == null)
            return;

        overlayLayer.Children.Remove(_host);
        _host.Content = null;
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