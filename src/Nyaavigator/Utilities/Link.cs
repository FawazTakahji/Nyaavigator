using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FluentAvalonia.UI.Controls;
using Nyaavigator.Enums;
using Nyaavigator.Builders;
using Nyaavigator.Models;
using NotificationType = Avalonia.Controls.Notifications.NotificationType;

namespace Nyaavigator.Utilities;

internal static class Link
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static void Open(string link)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            WindowsOpen(link);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (!LinuxXdgOpen(link))
                LinuxOpen(link);
        }
        else
            ShowCopyDialog(link);
    }

    private static void WindowsOpen(string link)
    {
        try
        {
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to open the link \"{link}\".");
            ShowCopyDialog(link);
        }
    }

    private static bool LinuxXdgOpen(string link)
    {
        try
        {
            Process.Start("xdg-open", link);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to open the link \"{link}\" with \"xdg-open\".");
            return false;
        }

        return true;
    }

    private static void LinuxOpen(string link)
    {
        try
        {
            Process.Start("open", link);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to open the link \"{link}\" with \"open\".");
            ShowCopyDialog(link);
        }
    }

    private static void ShowCopyDialog(string link)
    {
        TaskDialogCommand copyLinkButton = new()
        {
            Text = "Copy Link",
            IconSource = new SymbolIconSource { Symbol = Symbol.Link },
            Description = link,
            ClosesOnInvoked = true
        };
        copyLinkButton.Click += async (_, _) =>
        {
            await Clipboard.Copy(link);
            new Notification("Link Copied", type: NotificationType.Success).Send();
        };

        Dialog.Create()
            .Type(DialogType.Error)
            .Content("Unable to open the link on your current platform.")
            .Commands([copyLinkButton])
            .ShowAndForget();
    }
}