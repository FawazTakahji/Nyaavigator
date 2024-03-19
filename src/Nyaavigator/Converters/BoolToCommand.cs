using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.Input;

namespace Nyaavigator.Converters;

public class BoolToCommand : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IAsyncRelayCommand command)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return command.IsRunning ? command.CreateCancelCommand() : command;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}