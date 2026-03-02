using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Services;
using Nyaavigator.Core.Settings;

namespace Nyaavigator.Core.ViewModels;

public partial class SettingsViewModel : NavigationViewModelBase
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

    public override void OnNavigatedFrom()
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

    public override string Title { get; } = "Settings";
}