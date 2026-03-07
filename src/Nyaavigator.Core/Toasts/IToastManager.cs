namespace Nyaavigator.Core.Toasts;

public interface IToastManager
{
    public void Show(string content,
        ToastType type = ToastType.None,
        TimeSpan? expiration = null,
        bool showClose = false,
        Action? onClick = null,
        Action? onClose = null);
}