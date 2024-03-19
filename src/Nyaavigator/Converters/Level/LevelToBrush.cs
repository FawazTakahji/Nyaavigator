using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Nyaavigator.Converters.Level;

public class LevelToBrush : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
            if (value is not int level)
                return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

            switch (level)
            {
                case 0:
                case 1:
                    return new SolidColorBrush(Colors.Green);
                case 2:
                    return new SolidColorBrush(Colors.CornflowerBlue);
                case 3:
                    return new SolidColorBrush(Colors.Orange);
                case 4:
                case 5:
                    return new SolidColorBrush(Colors.Crimson);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
            throw new NotImplementedException();
        }
}