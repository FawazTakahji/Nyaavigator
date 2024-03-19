using CommunityToolkit.Mvvm.Messaging.Messages;
using Nyaavigator.Models;

namespace Nyaavigator.Messages;

public class NotificationMessage(Notification value) : ValueChangedMessage<Notification>(value);
public class NotificationLogsMessage(Notification value) : ValueChangedMessage<Notification>(value);
public class InfoViewMessage(string value) : ValueChangedMessage<string>(value);