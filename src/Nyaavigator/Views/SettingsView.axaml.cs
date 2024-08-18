using Avalonia.Interactivity;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using Material.Icons;
using Nyaavigator.ViewModels;

namespace Nyaavigator.Views;

public partial class SettingsView : DialogViewBase
{
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

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        CloseButton.Focus();
    }
}