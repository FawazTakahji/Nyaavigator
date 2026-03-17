using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Nyaavigator.Core.Nyaa;

namespace Nyaavigator.AvaloniaUI.Views.Search;

public class SubcategoriesConverter : IValueConverter
{
    public static readonly SubcategoriesConverter Instance = new();

    // TODO: find a better way to add an All button
    // empty object is used to represent "All" category using a data template
    private static readonly EmptyObject EmptyObject = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Category category)
        {
            return BindingOperations.DoNothing;
        }

        List<Category>? sourceCategories = category.SubCategories ?? category.Parent?.SubCategories;
        if (sourceCategories == null || sourceCategories.Count == 0)
        {
            return new List<object?>
            {
                EmptyObject
            };
        }

        List<object?> result = new(sourceCategories.Count + 1)
        {
            EmptyObject
        };

        result.AddRange(sourceCategories);
        return result;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EmptyObject;