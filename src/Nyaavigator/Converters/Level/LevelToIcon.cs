using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Material.Icons;

namespace Nyaavigator.Converters.Level;

public class LevelToIcon : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
            if (value is not int level)
                return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            return level switch
            {
                0 => MaterialIconKind.CodeBraces,
                1 => MaterialIconKind.Spider,
                2 => MaterialIconKind.AlertCircle,
                3 => MaterialIconKind.Alert,
                4 => MaterialIconKind.Cancel,
                5 => MaterialIconKind.SkullOutline,
                _ => MaterialIconKind.Help
            };
        }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
            throw new NotImplementedException();
        }
}