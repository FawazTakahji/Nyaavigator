using Avalonia;
using Avalonia.Controls.Primitives;

namespace Nyaavigator.AvaloniaUI.Controls;

public class ViewHeader : TemplatedControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ViewHeader, string?>(nameof(Title));

    public static readonly StyledProperty<object?> LeftContentProperty =
        AvaloniaProperty.Register<ViewHeader, object?>(nameof(LeftContent));

    public static readonly StyledProperty<object?> RightContentProperty =
        AvaloniaProperty.Register<ViewHeader, object?>(nameof(RightContent));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public object? LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }
}