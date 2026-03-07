using Microsoft.Extensions.DependencyInjection;
using Nyaavigator.AvaloniaUI.Dialog;
using Nyaavigator.AvaloniaUI.Services;
using Nyaavigator.AvaloniaUI.Toasts;
using Nyaavigator.Core.Dialog;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Toasts;

namespace Nyaavigator.AvaloniaUI.Extensions;

public static class UiServices
{
    public static IServiceCollection AddUiServices(this IServiceCollection services)
    {
        return services.AddSingleton<IAppManager, AppManager>()
            .AddSingleton<IDialogManager, DialogManager>()
            .AddSingleton<IToastManager, ToastManager>();
    }
}