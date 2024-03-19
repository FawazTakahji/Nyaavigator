using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using Material.Icons;
using Nyaavigator.Enums;

namespace Nyaavigator.Builders;

public class Dialog(TopLevel? xamlRoot = null)
{
    private readonly TopLevel _xamlRoot = xamlRoot ?? App.TopLevel;
    private string? _title;
    private string? _header;
    private IBrush? _headerForeground;
    private string? _subHeader;
    private object? _content;
    private IconSource? _iconSource;
    private IBrush? _iconForeground;
    private IList<TaskDialogButton>? _buttons = [TaskDialogButton.OKButton];
    private IList<TaskDialogCommand>? _commands;
    private bool _showHosted = true;

    public static Dialog Create(TopLevel? xamlRoot = null)
    {
        return new Dialog(xamlRoot);
    }

    public async Task<TaskDialogStandardResult> Show()
    {
        TaskDialog dialog = new()
        {
            XamlRoot = _xamlRoot,
            Title = _title,
            Header = _header,
            HeaderForeground = _headerForeground,
            SubHeader = _subHeader,
            Content = _content,
            IconSource = _iconSource,
            IconForeground = _iconForeground,
            Buttons = _buttons,
            Commands = _commands
        };

        return (TaskDialogStandardResult)await dialog.ShowAsync(_showHosted);
    }

    public void ShowAndForget()
    {
        TaskDialog dialog = new()
        {
            XamlRoot = _xamlRoot,
            Title = _title,
            Header = _header,
            HeaderForeground = _headerForeground,
            SubHeader = _subHeader,
            Content = _content,
            IconSource = _iconSource,
            IconForeground = _iconForeground,
            Buttons = _buttons,
            Commands = _commands
        };

        dialog.ShowAsync(_showHosted).SafeFireAndForget();
    }

    public Dialog Type(DialogType type)
    {
        _header = type switch
        {
            DialogType.Info => "Info",
            DialogType.Warning => "Warning",
            DialogType.Error => "Error",
            DialogType.Fatal => "Fatal Error",
            _ => string.Empty
        };
        _iconSource = new PathIconSource
        {
            Data = type switch
            {
                DialogType.Info => Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.AlertCircle)),
                DialogType.Warning => Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Alert)),
                DialogType.Error => Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Cancel)),
                DialogType.Fatal => Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.SkullOutline)),
                _ => Geometry.Parse(MaterialIconDataProvider.GetData(MaterialIconKind.Help))
            }
        };
        IBrush brush = type switch
        {
            DialogType.Info => Brushes.CornflowerBlue,
            DialogType.Warning => Brushes.Orange,
            DialogType.Error => Brushes.Crimson,
            DialogType.Fatal => Brushes.Crimson,
            _ => Brushes.White
        };
        _iconForeground = brush;
        _headerForeground = brush;

        return this;
    }

    public Dialog Title(string title)
    {
        _title = title;
        return this;
    }

    public Dialog Header(string header)
    {
        _header = header;
        return this;
    }

    public Dialog HeaderForeground(IBrush brush)
    {
        _headerForeground = brush;
        return this;
    }

    public Dialog SubHeader(string subHeader)
    {
        _subHeader = subHeader;
        return this;
    }

    public Dialog Content(object content)
    {
        _content = content;
        return this;
    }

    public Dialog Icon(IconSource iconSource)
    {
        _iconSource = iconSource;
        return this;
    }

    public Dialog IconForeground(IBrush brush)
    {
        _iconForeground = brush;
        return this;
    }

    public Dialog Buttons(IList<TaskDialogButton> buttons)
    {
        _buttons = buttons;
        return this;
    }

    public Dialog Commands(IList<TaskDialogCommand> commands)
    {
        _commands = commands;
        return this;
    }

    public Dialog NewWindow()
    {
        _showHosted = false;
        return this;
    }
}