using Avalonia.Controls;

namespace Nyaavigator.AvaloniaUI.Windows;

public class WindowSettings
{
    public int X { get; init; }
    public int Y { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
    public WindowState State { get; init; }
}