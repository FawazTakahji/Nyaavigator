using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Nyaavigator.AvaloniaUI.Converters;

public class ObjectConverters
{
    public static readonly EqualConverter Equal = new();
}

public class EqualConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [{ } a, { } b])
        {
            return BindingOperations.DoNothing;
        }

        return a.Equals(b);
    }
}