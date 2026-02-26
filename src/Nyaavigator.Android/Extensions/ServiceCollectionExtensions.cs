using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.Android.Storage;
using Nyaavigator.Core.Storage;

namespace Nyaavigator.Android.Extensions;

public static class AndroidServices
{
    public static IServiceCollection AddAndroidServices(this IServiceCollection services)
    {
        return services.AddSingleton<IPersistentStorageService, PersistentStorageService>();
    }
}