using System;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using Nyaavigator.Messages;

namespace Nyaavigator.Models;

public class Notification : Avalonia.Controls.Notifications.Notification
{
    public new string? Title { get; private set; }
    public new string? Message { get; private set; }
    public new NotificationType Type { get; private set; }
    public new TimeSpan Expiration { get; private set; }
    public new Action? OnClick { get; private set; }
    public new Action? OnClose { get; private set; }

    public Notification(string? title = null,
        string? message = null,
        NotificationType type = NotificationType.Information,
        TimeSpan? expiration = null,
        Action? onClick = null,
        Action? onClose = null) : base(title, message, type, expiration, onClick, onClose)
    {
        Title = title;
        Message = message;
        Type = type;
        Expiration = expiration.HasValue ? expiration.Value : TimeSpan.FromSeconds(3);
        OnClick = onClick;
        OnClose = onClose;
    }

    public void Send()
    {
        WeakReferenceMessenger.Default.Send(new NotificationMessage(this));
    }

    public void SendToLogsWindow()
    {
        WeakReferenceMessenger.Default.Send(new NotificationLogsMessage(this));
    }
}