using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;

namespace Nyaavigator.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigationService;
    public SettingsService SettingsService { get; set; }
    private readonly IAppManager _appManager;

    public SettingsViewModel(NavigationService navigationService, SettingsService settingsService, IAppManager appManager)
    {
        _navigationService = navigationService;
        SettingsService = settingsService;
        _appManager = appManager;
    }

    [RelayCommand]
    private void GoSearch()
    {
        _navigationService.NavigateTo<SearchViewModel>();
    }

    [RelayCommand]
    private void SetTheme(Theme theme)
    {
        SettingsService.Settings.Theme = theme;
        _appManager.SetTheme(theme);
    }

    [RelayCommand]
    private void Save()
    {
        SettingsService.Save();
    }

    public string Title { get; } = "Settings";
}