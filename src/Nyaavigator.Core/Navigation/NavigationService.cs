using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Nyaavigator.Core.Navigation;

public partial class NavigationService : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    [ObservableProperty]
    private INavigable? _current;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TNavigable>() where TNavigable : INavigable
    {
        Current = _serviceProvider.GetRequiredService<TNavigable>();
    }
}