using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Nyaavigator.Core.Nyaa;

namespace Nyaavigator.AvaloniaUI.Views.Search;

public class IsCategorySelectedConverter : IMultiValueConverter
{
    public static readonly IsCategorySelectedConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [Category selectedCategory, Category category])
        {
            return BindingOperations.DoNothing;
        }

        return selectedCategory == category || selectedCategory.Parent == category;
    }
}