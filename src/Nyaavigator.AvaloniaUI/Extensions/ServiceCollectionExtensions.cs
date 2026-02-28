using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.AvaloniaUI.Services;
using Nyaavigator.Core.Services;

namespace Nyaavigator.AvaloniaUI.Extensions;

public static class UiServices
{
    public static IServiceCollection AddUiServices(this IServiceCollection services)
    {
        return services.AddSingleton<IAppManager, AppManager>();
    }
}