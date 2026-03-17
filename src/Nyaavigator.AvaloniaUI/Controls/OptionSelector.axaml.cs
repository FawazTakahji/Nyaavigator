using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Nyaavigator.AvaloniaUI.Controls;

public class OptionSelector : TemplatedControl
{
    public static readonly StyledProperty<string> HeaderProperty = AvaloniaProperty.Register<OptionSelector, string>(
        nameof(Header));

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<OptionSelector, IEnumerable?>(
        nameof(ItemsSource));

    public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<OptionSelector, object?>(
        nameof(SelectedItem));

    public static readonly StyledProperty<ICommand?> SelectionCommandProperty = AvaloniaProperty.Register<OptionSelector, ICommand?>(
        nameof(SelectionCommand));

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<OptionSelector, IDataTemplate?>(
        nameof(ItemTemplate));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public ICommand? SelectionCommand
    {
        get => GetValue(SelectionCommandProperty);
        set => SetValue(SelectionCommandProperty, value);
    }

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    private ScrollViewer? _scrollViewer;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _scrollViewer?.PointerWheelChanged -= OnScrollViewerPointerWheelChanged;
        base.OnApplyTemplate(e);

        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        _scrollViewer?.PointerWheelChanged += OnScrollViewerPointerWheelChanged;

        AddHandler(ToggleButton.IsCheckedChangedEvent, OnIsCheckedChanged, RoutingStrategies.Bubble);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_scrollViewer is not null)
        {
            _scrollViewer.PointerWheelChanged -= OnScrollViewerPointerWheelChanged;
            _scrollViewer = null;
        }

        RemoveHandler(ToggleButton.IsCheckedChangedEvent, OnIsCheckedChanged);
    }

    private void OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not ToggleButton { IsChecked: false } toggleButton)
        {
            return;
        }

        if (Equals(toggleButton.DataContext, SelectedItem))
        {
            toggleButton.IsChecked = true;
        }
    }

    private void OnScrollViewerPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
        {
            return;
        }

        if (e.Delta.Y > 0)
        {
            scrollViewer.PageLeft();
        }
        else
        {
            scrollViewer.PageRight();
        }
    }
}