using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Nyaavigator.Converters.CategoryIcon;

public class StringToBitmap : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string category)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        Uri uri = new($"avares://Nyaavigator/Assets/Categories/{category}.png");
        if (AssetLoader.Exists(uri))
            return new Bitmap(AssetLoader.Open(uri));

        return new Bitmap(AssetLoader.Open(new Uri("avares://Nyaavigator/Assets/Categories/Unknown.png")));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}