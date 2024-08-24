using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using NLog;
using Nyaavigator.Models;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace Nyaavigator.Utilities;

public static class Window
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private const int MinMargin = 100;

    public static void SaveLocation(this AvaloniaWindow window)
    {
        if (window.WindowState == WindowState.Minimized)
            return;

        string filePath = Path.Combine(App.BaseDirectory, "Window.json");
        WindowLocation windowLocation = LoadLocation(filePath) ?? new();

        if (window.WindowState == WindowState.Normal)
        {
            windowLocation.Left = window.Position.X;
            windowLocation.Top = window.Position.Y;
            windowLocation.Width = (int)window.Width;
            windowLocation.Height = (int)window.Height;
        }
        windowLocation.WindowState = window.WindowState;

        try
        {
            string json = JsonSerializer.Serialize(windowLocation, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Couldn't save window location.");
        }
    }

    public static void RestoreLocation(this AvaloniaWindow window)
    {
        string filePath = Path.Combine(App.BaseDirectory, "Window.json");
        WindowLocation? windowLocation = LoadLocation(filePath);
        if (windowLocation is null)
            return;
        Screen? screen = window.Screens.ScreenFromWindow(window);
        if (screen is null)
        {
            window.WindowState = windowLocation.WindowState ?? window.WindowState;
            return;
        }

        bool isPositionValid = windowLocation is { Left: not null, Top: not null }
                               && windowLocation.Left.Value <= screen.WorkingArea.Width - MinMargin
                               && windowLocation.Top.Value <= screen.WorkingArea.Height - MinMargin;
        bool isSizeValid = windowLocation is { Width: not null, Height: not null }
                           && windowLocation.Width.Value <= screen.WorkingArea.Width
                           && windowLocation.Height.Value <= screen.WorkingArea.Height;

        if (isPositionValid)
            window.Position = new PixelPoint(windowLocation.Left!.Value, windowLocation.Top!.Value);
        if (isSizeValid)
        {
            window.Width = windowLocation.Width!.Value;
            window.Height = windowLocation.Height!.Value;
        }
        window.WindowState = windowLocation.WindowState ?? window.WindowState;
    }

    private static WindowLocation? LoadLocation(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Warn("Couldn't load window location, file doesn't exist.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<WindowLocation>(json, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Couldn't load window location.");
            return null;
        }
    }
}