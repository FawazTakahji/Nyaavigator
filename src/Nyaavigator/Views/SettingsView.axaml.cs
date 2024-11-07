using Avalonia.Input;
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

        QBittorrentHostExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Server)) };
        QBittorrentPortExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Ethernet)) };
        QBittorrentProtocolExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Protocol)) };
        QBittorrentUsernameExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Person)) };
        QBittorrentPasswordExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Key)) };
        QBittorrentFolderExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Folder)) };
        QBittorrentCategoryExpanderItem.IconSource = new PathIconSource { Data = Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Category)) };
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        CloseButton.Focus();
    }

    private void QBittorrentTagTextBox_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled || e.Key != Key.Enter || DataContext is not SettingsViewModel vm)
            return;

        if (vm.QBittorrentAddTagCommand.CanExecute(null))
            vm.QBittorrentAddTagCommand.Execute(null);
    }
}