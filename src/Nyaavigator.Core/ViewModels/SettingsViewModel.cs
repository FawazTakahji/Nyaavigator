using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;

namespace Nyaavigator.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    public NavigationService NavigationService { get; }
    public SettingsService SettingsService { get; }
    private readonly IAppManager _appManager;

    public SettingsViewModel(NavigationService navigationService, SettingsService settingsService, IAppManager appManager)
    {
        NavigationService = navigationService;
        SettingsService = settingsService;
        _appManager = appManager;
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
        try
        {
            Save();
        }
        catch (Exception e)
        {
            // TODO: add logging
        }
    }
}