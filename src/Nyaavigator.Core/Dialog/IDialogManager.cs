namespace Nyaavigator.Core.Dialog;

public interface IDialogManager
{
    public void Show(string? title, string? message, DialogButton buttons = DialogButton.OK, DialogMode mode = DialogMode.None);
    public Task<DialogResult> ShowModal(string? title, string? message, DialogButton buttons = DialogButton.OK, DialogMode mode = DialogMode.None);
}