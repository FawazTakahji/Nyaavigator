using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.ViewModels;

namespace Nyaavigator.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>();
        AccentExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.PaletteSwatchOutline)) };
        LogsExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.TextBoxOutline)) };
        FolderExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Folder)) };
        UpdatesExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Sync)) };
        RepoExpander.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Github)) };
    }

    private void CloseView(object? sender, RoutedEventArgs e)
    {
        if (this.GetLogicalParent() is FlyoutPresenter presenter && presenter.GetLogicalParent() is Popup popup)
            popup.Close();
    }
}