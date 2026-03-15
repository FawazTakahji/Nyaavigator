using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Irihi.Avalonia.Shared.Helpers;

namespace Nyaavigator.AvaloniaUI.Behaviors;

public class ScrollViewerFadeEdge : AvaloniaObject
{
    public static readonly AttachedProperty<double> FadedEdgeProperty =
        AvaloniaProperty.RegisterAttached<ScrollViewerFadeEdge, ScrollViewer, double>("FadedEdge", 20);

    public static void SetFadedEdge(ScrollViewer obj, double value) => obj.SetValue(FadedEdgeProperty, value);
    public static double GetFadedEdge(ScrollViewer obj) => obj.GetValue(FadedEdgeProperty);

    static ScrollViewerFadeEdge()
    {
        FadedEdgeProperty.Changed.AddClassHandler<ScrollViewer>(OnFadedEdgeChanged);
    }

    private static void OnFadedEdgeChanged(ScrollViewer scrollViewer, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is double and > 0)
        {
            UpdateMask(scrollViewer);

            scrollViewer.GetObservable(ScrollViewer.OffsetProperty).Subscribe(_ => UpdateMask(scrollViewer));
            scrollViewer.GetObservable(ScrollViewer.ExtentProperty).Subscribe(_ => UpdateMask(scrollViewer));
            scrollViewer.GetObservable(Visual.BoundsProperty).Subscribe(_ => UpdateMask(scrollViewer));
        }
    }

    private static void UpdateMask(ScrollViewer scrollViewer)
    {
        double thickness = GetFadedEdge(scrollViewer);
        if (thickness < 1)
        {
            return;
        }

        double leftOpacity = scrollViewer.Offset.X <= 1 ? 1.0 : 0.0;
        double rightMax = scrollViewer.Extent.Width - scrollViewer.Viewport.Width;
        if (rightMax < 1)
        {
            scrollViewer.OpacityMask = null;
            return;
        }

        double rightOpacity = scrollViewer.Offset.X >= rightMax - 1 ? 1.0 : 0.0;

        double startFade = thickness / scrollViewer.Bounds.Width;
        double endFade = 1.0 - thickness / scrollViewer.Bounds.Width;

        scrollViewer.OpacityMask = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            GradientStops =
            [
                new GradientStop(Color.FromArgb((byte)(leftOpacity * 255), 0, 0, 0), 0),
                new GradientStop(Colors.Black, startFade),
                new GradientStop(Colors.Black, endFade),
                new GradientStop(Color.FromArgb((byte)(rightOpacity * 255), 0, 0, 0), 1)
            ]
        };
    }
}