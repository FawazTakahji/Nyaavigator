using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.AvaloniaUI.Windows;

public static class WindowHelper
{
    private const string FileName = "window.json";
    private static IPersistentStorageService? _storage;
    private static ILogger? _logger;

    public static void SaveState(Window window)
    {
        TryGetLogger()?.LogInformation("Saving window state");
        try
        {
            GetStorageService();

            WindowSettings settings = new()
            {
                X = window.Position.X,
                Y = window.Position.Y,
                Width = window.Width,
                Height = window.Height,
                State = window.WindowState
            };

            string json = JsonSerializer.Serialize(settings);
            _storage.Write(FileName, json);
        }
        catch (Exception e)
        {
            TryGetLogger()?.LogError(e, "Failed to save window state");
        }
    }

    public static void LoadState(Window window)
    {
        TryGetLogger()?.LogInformation("Loading window state");
        try
        {
            GetStorageService();

            string? json = _storage.Read(FileName);
            if (json is null)
            {
                return;
            }
            WindowSettings? settings = JsonSerializer.Deserialize<WindowSettings>(json);
            if (settings is null)
            {
                TryGetLogger()?.LogWarning("Could not deserialize window settings");
                return;
            }

            Screen? screen = window.Screens.ScreenFromWindow(window);
            if (settings.State == WindowState.Normal)
            {
                if (screen is not null)
                {
                    const int margin = 25;

                    int minX = screen.WorkingArea.X - ((int)settings.Width - margin);
                    int maxX = screen.WorkingArea.Right - margin;
                    int maxY = screen.WorkingArea.Bottom - margin;

                    int clampedX = Math.Clamp(settings.X, minX, maxX);
                    int clampedY = Math.Clamp(settings.Y, screen.WorkingArea.Y, maxY);

                    window.Position = new PixelPoint(clampedX, clampedY);
                }
                else
                {
                    window.Position = new PixelPoint(settings.X, settings.Y);
                    TryGetLogger()?.LogWarning("Could not get screen containing window");
                }
                window.Width = settings.Width;
                window.Height = settings.Height;
            }

            window.WindowState = settings.State == WindowState.Minimized ? WindowState.Normal : settings.State;
        }
        catch (Exception e)
        {
            TryGetLogger()?.LogError(e, "Failed to load window state");
        }
    }

    [MemberNotNull(nameof(_storage))]
    private static void GetStorageService()
    {
        if (_storage is not null)
        {
            return;
        }

        _storage = Ioc.Default.GetRequiredService<IPersistentStorageService>();
    }

    private static ILogger? TryGetLogger()
    {
        _logger ??= Ioc.Default.GetService<ILoggerFactory>()?.CreateLogger(typeof(WindowHelper));
        return _logger;
    }
}