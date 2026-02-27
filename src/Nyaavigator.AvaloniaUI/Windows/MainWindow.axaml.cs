using System;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;

namespace Nyaavigator.AvaloniaUI.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        KeyBindings.Add(new()
        {
            Gesture = new KeyGesture(Key.F1),
            Command = new RelayCommand(() =>
            {
                Width = 1200;
                Height = 700;
            })
        });
        KeyBindings.Add(new()
        {
            Gesture = new KeyGesture(Key.F2),
            Command = new RelayCommand(() =>
            {
                Width = 500;
                Height = 900;
            })
        });
#endif
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        WindowHelper.LoadState(this);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        WindowHelper.SaveState(this);
    }
}