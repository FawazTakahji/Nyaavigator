using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Nyaavigator.AvaloniaUI.Views;
using Nyaavigator.AvaloniaUI.Views.Settings;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.AvaloniaUI;

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<INavigable, Control> _cache = new();

    public Control? Build(object? param)
    {
        if (param is not INavigable navigable)
        {
            return null;
        }
        if (_cache.TryGetValue(navigable, out Control? control))
        {
            return control;
        }

        Control? view = navigable switch
        {
            SearchViewModel _ => new SearchView(),
            SettingsViewModel _ => new SettingsView(),
            _ => null
        };

        if (view is not null)
        {
            view.DataContext = navigable;
            view.Unloaded += View_Unloaded;
            _cache.Add(navigable, view);
        }

        return view;
    }

    public bool Match(object? data)
    {
        return data is INavigable;
    }

    private void View_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: INavigable navigable })
        {
            navigable.OnNavigatedFrom();
        }
    }
}