using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.Settings;
using Nyaavigator.Core.ViewModels;

namespace Nyaavigator.Core.Extensions;

public static class CoreServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCoreServices()
        {
            return services.AddHttpClients()

                .AddSingleton<MainViewModel>()
                .AddSingleton<SearchViewModel>()
                .AddSingleton<SettingsViewModel>()

                .AddSingleton<NavigationService>()
                .AddSingleton<SettingsService>();
        }

        private IServiceCollection AddHttpClients()
        {
            services.AddHttpClient("nyaa", client =>
                {
                    client.DefaultRequestHeaders.Accept.ParseAdd("text/html; charset=UTF-8");
                    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            return services;
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        public void DisposeLogProviders()
        {
            IEnumerable<ILoggerProvider> providers = serviceProvider.GetServices<ILoggerProvider>();
            foreach (ILoggerProvider provider in providers)
            {
                provider.Dispose();
            }
        }
    }
}