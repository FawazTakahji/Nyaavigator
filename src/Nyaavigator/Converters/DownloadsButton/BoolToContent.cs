using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Material.Icons;

namespace Nyaavigator.Converters.DownloadsButton;

public class BoolToContent : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        if (targetType.IsAssignableTo(typeof(string)))
            return boolValue ? "Cancel Downloads" : "Download Selected";

        if (targetType.IsAssignableTo(typeof(MaterialIconKind)))
            return boolValue ? MaterialIconKind.CancelThick : MaterialIconKind.Download;

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}