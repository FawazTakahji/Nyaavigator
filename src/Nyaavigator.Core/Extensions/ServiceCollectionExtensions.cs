using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.Core.Extensions;

public static class CoreServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<SearchViewModel>();
        services.AddSingleton<SettingsViewModel>();

        services.AddSingleton<NavigationService>();
        return services;
    }
}