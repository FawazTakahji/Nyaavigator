using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Markdown.Avalonia;
using Nyaavigator.Commands;
using Nyaavigator.Converters;
using Nyaavigator.Messages;
using Nyaavigator.Models;
using Nyaavigator.ViewModels;

namespace Nyaavigator.Views;

public partial class TorrentInfoView : DialogViewBase, IRecipient<InfoViewMessage>
{
    public TorrentInfoView(TorrentInfoViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        WeakReferenceMessenger.Default.Register<InfoViewMessage>(this);
        CloseButton.Click += (_, _) => Hide();
        ScrollButton.Click += (_, _) => { ScrollViewer.ScrollToHome(); };
    }

    protected override void Hide()
    {
        if (_lastFocus != null)
        {
            _lastFocus.Focus();
            _lastFocus = null;
        }

        var viewModel = (TorrentInfoViewModel)DataContext;
        if (viewModel.GetInfoCancelCommand.CanExecute(null))
            viewModel.GetInfoCancelCommand.Execute(null);
        DataContext = null;

        OverlayLayer? overlayLayer = OverlayLayer.GetOverlayLayer(_host);
        overlayLayer?.Children.Remove(_host);
        _host.Content = null;

        _tcs?.TrySetResult();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        CloseButton.Focus();
    }

    public void Receive(InfoViewMessage message)
    {
        Flyout flyout = new()
        {
            Content = new TextBlock
            {
                Text = message.Value,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            Placement = PlacementMode.Bottom,
            VerticalOffset = -100,
            PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.None
        };
        flyout.ShowAt(this);
        DispatcherTimer.RunOnce(() => flyout.Hide(), TimeSpan.FromSeconds(3));
    }

    private void ExpandItem(object? sender, SelectionChangedEventArgs e)
    {
        var tree = (TreeView)sender;
        if (((ListItem)tree.SelectedItem).Children.Count > 0)
        {
            var item = (TreeViewItem)tree.TreeContainerFromItem(tree.SelectedItem);
            item.IsExpanded = !item.IsExpanded;
        }

        tree.SelectedItem = null;
    }

    private void SetHyperlinkCommand(object? sender, EventArgs e)
    {
        if (sender is not MarkdownScrollViewer md)
            return;

        md.Engine.HyperlinkCommand = new OpenLinkCommand();
    }

    private void SetupScrollViewer(object? sender, TemplateAppliedEventArgs e)
    {
        ScrollBar? scrollBar = e.NameScope.Find<ScrollBar>("PART_VerticalScrollBar");
        if (scrollBar == null)
            return;

        scrollBar.Classes.Add("AlwaysExpanded");

        Binding buttonBinding = new()
        {
            Source = scrollBar,
            Path = nameof(scrollBar.IsVisible),
            Mode = BindingMode.OneWay
        };
        ScrollButton.Bind(IsVisibleProperty, buttonBinding);

        Binding paddingBinding = new()
        {
            Source = scrollBar,
            Path = nameof(scrollBar.IsVisible),
            Converter = new ScrollViewerPadding(),
            Mode = BindingMode.OneWay
        };
        ScrollViewer.Bind(PaddingProperty, paddingBinding);
    }

    // for previewer
#if DEBUG
    public TorrentInfoView()
    {
        InitializeComponent();
        DataContext = new TorrentInfoViewModel(Utilities.UI.GetFakeInfo());
        WeakReferenceMessenger.Default.Register<InfoViewMessage>(this);
        CloseButton.Click += (_, _) => Hide();
        ScrollButton.Click += (_, _) => { ScrollViewer.ScrollToHome(); };
    }
#endif
}