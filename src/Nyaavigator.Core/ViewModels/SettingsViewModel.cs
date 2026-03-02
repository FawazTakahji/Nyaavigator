using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;

namespace Nyaavigator.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    public SettingsService SettingsService { get; set; }
    private readonly IAppManager _appManager;

    public SettingsViewModel(SettingsService settingsService, IAppManager appManager)
    {
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

    public string Title { get; } = "Settings";
}