using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Nyaavigator.Converters.CategoryIcon;

public class StringToDescription : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string category)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return category switch
        {
            "1_1" => "Anime - AMV",
            "1_2" => "Anime - English Translated",
            "1_3" => "Anime - Non English Translated",
            "1_4" => "Anime - Raw",

            "2_1" => "Audio - Lossless",
            "2_2" => "Audio - Lossy",

            "3_1" => "Literature - English Translated",
            "3_2" => "Literature - Non English Translated",
            "3_3" => "Literature - Raw",

            "4_1" => "Live Action - English Translated",
            "4_2" => "Live Action - Idol/PV",
            "4_3" => "Live Action - Non English Translated",
            "4_4" => "Live Action - Raw",

            "5_1" => "Pictures - Graphics",
            "5_2" => "Pictures - Photos",

            "6_1" => "Software - Applications",
            "6_2" => "Software - Games",

            _ => "Unknown"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}