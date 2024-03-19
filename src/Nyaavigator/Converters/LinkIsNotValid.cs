using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Nyaavigator.Converters;

public class LinkIsNotValid : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string link)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return !Uri.IsWellFormedUriString(link, UriKind.Absolute);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}