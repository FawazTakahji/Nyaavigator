using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Interactivity;

namespace Nyaavigator.AvaloniaUI;

public static class BackRequestedHandler
{
    public static event EventHandler<RoutedEventArgs>? GlobalDialogBackRequested;
    private static readonly List<EventHandler<RoutedEventArgs>> Handlers = [];
    private static readonly Lock Lock = new();

    public static void Initialize()
    {
        App.TopLevel?.BackRequested += OnBackRequested;
    }

    public static void Subscribe(EventHandler<RoutedEventArgs> handler)
    {
        lock (Lock)
        {
            Handlers.Add(handler);
        }
    }

    public static void Unsubscribe(EventHandler<RoutedEventArgs> handler)
    {
        lock (Lock)
        {
            Handlers.Remove(handler);
        }
    }

    private static void OnBackRequested(object? sender, RoutedEventArgs e)
    {
        GlobalDialogBackRequested?.Invoke(sender, e);

        List<EventHandler<RoutedEventArgs>> snapshot;
        lock (Lock)
        {
            if (Handlers.Count < 1)
            {
                return;
            }
            snapshot = new List<EventHandler<RoutedEventArgs>>(Handlers);
        }

        for (int i = snapshot.Count - 1; i >= 0; i--)
        {
            snapshot[i].Invoke(sender, e);
        }
    }
}