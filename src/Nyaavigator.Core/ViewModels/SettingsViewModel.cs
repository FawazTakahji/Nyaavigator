using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;
using Nyaavigator.Core.Toasts;

namespace Nyaavigator.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    private readonly ILogger<SettingsViewModel> _logger;
    public NavigationService NavigationService { get; }
    public SettingsService SettingsService { get; }
    private readonly IAppManager _appManager;
    private readonly IToastManager _toastManager;

    public SettingsViewModel(ILogger<SettingsViewModel> logger,
        NavigationService navigationService,
        SettingsService settingsService,
        IAppManager appManager,
        IToastManager toastManager)
    {
        _logger = logger;
        NavigationService = navigationService;
        SettingsService = settingsService;
        _appManager = appManager;
        _toastManager = toastManager;
    }

    [RelayCommand]
    private void SetTheme(Theme theme)
    {
        _appManager.SetTheme(theme);
    }

    private void Save()
    {
        SettingsService.Save();
    }

    public void OnNavigatedFrom()
    {
        _logger.LogInformation("Saving settings");
        try
        {
            Save();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save settings");
            _toastManager.Show("Could not save settings", ToastType.Error, showClose: true);
        }
    }
}