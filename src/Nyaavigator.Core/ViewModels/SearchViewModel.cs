using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Navigation;

namespace Nyaavigator.Core.ViewModels;

public partial class SearchViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigationService;

    public SearchViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void GoSettings()
    {
        _navigationService.Push<SettingsViewModel>();
    }
}