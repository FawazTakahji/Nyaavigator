using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Nyaavigator.AvaloniaUI.Controls;

// Source - https://stackoverflow.com/a/47908267
public class SortingWrapPanel : WrapPanel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (Control child in Children)
        {
            child.Measure(availableSize);
        }

        return DoLayout(availableSize.Width, false);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return DoLayout(finalSize.Width, true);
    }

    private Size DoLayout(double constraintWidth, bool isArrange)
    {
        double x = 0d;
        double y = 0d;
        double currentRowMaxHeight = 0d;
        double maxWidthReached = 0d;

        List<Control> remainingChildren = Children.ToList();
        while (remainingChildren.Count > 0)
        {
            Control? child = remainingChildren.FirstOrDefault(c =>
            {
                double reqWidth = (x == 0) ? c.DesiredSize.Width : c.DesiredSize.Width + ItemSpacing;
                return x + reqWidth <= constraintWidth;
            });

            if (child is null)
            {
                y += currentRowMaxHeight + LineSpacing;
                x = 0d;
                currentRowMaxHeight = 0d;

                child = remainingChildren.First();
            }

            if (x > 0)
            {
                x += ItemSpacing;
            }

            if (isArrange)
            {
                child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
            }

            x += child.DesiredSize.Width;
            currentRowMaxHeight = Math.Max(currentRowMaxHeight, child.DesiredSize.Height);
            maxWidthReached = Math.Max(maxWidthReached, x);

            remainingChildren.Remove(child);
        }

        return new Size(maxWidthReached, y + currentRowMaxHeight);
    }
}