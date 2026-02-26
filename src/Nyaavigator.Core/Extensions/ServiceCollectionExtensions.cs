using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.Core.Extensions;

public static class CoreServices
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services.AddSingleton<MainViewModel>()
            .AddSingleton<SearchViewModel>()
            .AddSingleton<SettingsViewModel>()

            .AddSingleton<NavigationService>();
    }
}