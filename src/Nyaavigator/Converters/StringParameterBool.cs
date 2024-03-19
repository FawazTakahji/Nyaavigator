using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Nyaavigator.Converters;

public class StringParameterBool : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string sorting || parameter is not string parameterString)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return sorting == parameterString;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isChecked || parameter is not string parameterString)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return isChecked ? parameterString : BindingOperations.DoNothing;
    }
}