using System;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Extensions;

namespace Nyaavigator.AvaloniaUI;

public class DesignServiceLocator : MarkupExtension
{
    public required Type Type { get; init; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Ioc.Default.GetRequiredService(Type);
    }

    public static void BuildProvider()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddCoreServices();
        Ioc.Default.ConfigureServices(collection.BuildServiceProvider());
    }
}