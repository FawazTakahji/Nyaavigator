using System;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Nyaavigator.AvaloniaUI;

public class DesignServiceLocator : MarkupExtension
{
    public required Type Type { get; init; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Ioc.Default.GetRequiredService(Type);
    }
}