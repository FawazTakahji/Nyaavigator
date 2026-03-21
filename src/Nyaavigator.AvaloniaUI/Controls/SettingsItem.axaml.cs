using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Nyaavigator.AvaloniaUI.Controls;

[PseudoClasses($"{PressedClass}, {ClickableClass}")]
public class SettingsItem : TemplatedControl
{
    private const string PressedClass = ":pressed";
    private const string ClickableClass = ":clickable";

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SettingsItem, string>(nameof(Header));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<SettingsItem, string?>(nameof(Description));

    public static readonly StyledProperty<StreamGeometry?> IconProperty =
        AvaloniaProperty.Register<SettingsItem, StreamGeometry?>(nameof(Icon));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<SettingsItem, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<SettingsItem, object?>(nameof(Content));

    public static readonly StyledProperty<object?> FooterContentProperty = AvaloniaProperty.Register<SettingsItem, object?>(
        nameof(FooterContent));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public StreamGeometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public object? FooterContent
    {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    private ContentPresenter? _contentPart;
    private ContentPresenter? _footerPart;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _contentPart = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        _footerPart = e.NameScope.Find<ContentPresenter>("PART_FooterPresenter");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CommandProperty)
        {
            PseudoClasses.Set(ClickableClass, change.NewValue is ICommand);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.Handled || _contentPart?.IsPointerOver == true || _footerPart?.IsPointerOver == true)
        {
            return;
        }

        if (Command != null && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            PseudoClasses.Add(PressedClass);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!Equals(e.Pointer.Captured, this))
        {
            return;
        }

        PseudoClasses.Remove(PressedClass);
        e.Pointer.Capture(null);

        if (new Rect(Bounds.Size).Contains(e.GetPosition(this))
            && Command?.CanExecute(null) == true)
        {
            Command.Execute(null);
        }

        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        PseudoClasses.Remove(PressedClass);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (Command != null && e.Key is Key.Enter or Key.Space)
        {
            PseudoClasses.Add(PressedClass);
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (Command != null && e.Key is Key.Enter or Key.Space)
        {
            if (PseudoClasses.Contains(PressedClass))
            {
                PseudoClasses.Remove(PressedClass);
                if (Command.CanExecute(null))
                {
                    Command.Execute(null);
                }
                e.Handled = true;
            }
        }
    }
}