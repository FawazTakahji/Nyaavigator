using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Nyaavigator.Core.Dialog;
using Ursa.Controls;
using DialogButton = Nyaavigator.Core.Dialog.DialogButton;
using DialogMode = Nyaavigator.Core.Dialog.DialogMode;
using DialogResult = Nyaavigator.Core.Dialog.DialogResult;
using UrsaDialogButton = Ursa.Controls.DialogButton;
using UrsaDialogMode = Ursa.Controls.DialogMode;
using UrsaDialogResult = Ursa.Controls.DialogResult;

namespace Nyaavigator.AvaloniaUI.Dialog;

public class DialogManager : IDialogManager
{
    public void Show(string? title, string? message, DialogButton buttons = DialogButton.OK, DialogMode mode = DialogMode.None)
    {
        OverlayDialog.Show(message, "Global", new OverlayDialogOptions
        {
            Title = title,
            Buttons = ConvertButton(buttons),
            Mode = ConvertMode(mode),
            StyleClass = "MessageBox"
        });
    }

    public async Task<DialogResult> ShowModal(string? title, string? message, DialogButton buttons = DialogButton.OK, DialogMode mode = DialogMode.None)
    {
        UrsaDialogResult result = await OverlayDialog.ShowModal(CreateTextBlock(message), null, "Global", new OverlayDialogOptions
        {
            Title = title,
            Buttons = ConvertButton(buttons),
            Mode = ConvertMode(mode),
            StyleClass = "MessageBox"
        });

        return result switch
        {
            UrsaDialogResult.OK => DialogResult.OK,
            UrsaDialogResult.Cancel => DialogResult.Cancel,
            UrsaDialogResult.Yes => DialogResult.Yes,
            UrsaDialogResult.No => DialogResult.No,
            _ => DialogResult.None
        };
    }

    private static SelectableTextBlock CreateTextBlock(string? message)
    {
        return new SelectableTextBlock
        {
            Text = message,
            Margin = new Thickness(16, 24),
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14,
            TextTrimming = TextTrimming.CharacterEllipsis,
            TextWrapping = TextWrapping.Wrap
        };
    }

    private static UrsaDialogButton ConvertButton(DialogButton button)
    {
        return button switch
        {
            DialogButton.OK => UrsaDialogButton.OK,
            DialogButton.OKCancel => UrsaDialogButton.OKCancel,
            DialogButton.YesNo => UrsaDialogButton.YesNo,
            DialogButton.YesNoCancel => UrsaDialogButton.YesNoCancel,
            _ => UrsaDialogButton.None
        };
    }

    private static UrsaDialogMode ConvertMode(DialogMode mode)
    {
        return mode switch
        {
            DialogMode.Success => UrsaDialogMode.Success,
            DialogMode.Info => UrsaDialogMode.Info,
            DialogMode.Warning => UrsaDialogMode.Warning,
            DialogMode.Error => UrsaDialogMode.Error,
            DialogMode.Question => UrsaDialogMode.Question,
            _ => UrsaDialogMode.None
        };
    }
}