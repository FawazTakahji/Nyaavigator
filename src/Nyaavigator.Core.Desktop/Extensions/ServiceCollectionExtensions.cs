using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Core.Desktop.Storage;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Core.Desktop.Extensions;

public static class DesktopServices
{
    public static IServiceCollection AddDesktopServices(this IServiceCollection services)
    {
        return services.AddSingleton<IPersistentStorageService, PersistentStorageService>();
    }
}