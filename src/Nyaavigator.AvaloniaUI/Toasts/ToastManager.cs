using System;
using System.Collections.Generic;
using Avalonia.Controls.Notifications;
using Nyaavigator.Core.Toasts;
using Ursa.Controls;
using IToastManager = Nyaavigator.Core.Toasts.IToastManager;

namespace Nyaavigator.AvaloniaUI.Toasts;

public class ToastManager : IToastManager
{
    private WindowToastManager? _manager;
    private Queue<(string content,
        ToastType type,
        TimeSpan? expiration,
        bool showClose,
        Action? onClick,
        Action? onClose)>? _toasts;

    public ToastManager()
    {
        if (App.TopLevel is { } topLevel)
        {
            _manager = new WindowToastManager(topLevel);
        }
        else
        {
            App.MainViewLoaded += OnMainViewLoaded;
        }
    }

    private void OnMainViewLoaded(object? sender, EventArgs e)
    {
        App.MainViewLoaded -= OnMainViewLoaded;
        _manager = new WindowToastManager(App.TopLevel);

        if (_toasts is null)
        {
            return;
        }
        while (_toasts.TryDequeue(out var toast))
        {
            _manager.Show(toast.content,
                ConvertToastType(toast.type),
                toast.expiration,
                false,
                toast.showClose,
                toast.onClick,
                toast.onClose,
                toast.type == ToastType.None ? null : ["Light"]);
        }
        _toasts = null;
    }

    public void Show(string content,
        ToastType type = ToastType.None,
        TimeSpan? expiration = null,
        bool showClose = true,
        Action? onClick = null,
        Action? onClose = null)
    {
        if (_manager is null)
        {
            if (App.TopLevel is not { } topLevel)
            {
                _toasts ??= [];
                _toasts.Enqueue((content, type, expiration, showClose, onClick, onClose));
                return;
            }

            _manager = new WindowToastManager(topLevel);
        }

        _manager.Show(content,
            ConvertToastType(type),
            expiration,
            false,
            showClose,
            onClick,
            onClose,
            type == ToastType.None ? null : ["Light"]);
    }

    private static NotificationType ConvertToastType(ToastType type)
    {
        return type switch
        {
            ToastType.Success => NotificationType.Success,
            ToastType.Warning => NotificationType.Warning,
            ToastType.Error => NotificationType.Error,
            _ => NotificationType.Information
        };
    }
}