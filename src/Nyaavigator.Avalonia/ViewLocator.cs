using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Nyaavigator.Avalonia.Views;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.Avalonia;

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
            _cache.Add(navigable, view);
        }

        return view;
    }

    public bool Match(object? data)
    {
        return data is INavigable;
    }
}