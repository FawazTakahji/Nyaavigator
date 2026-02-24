using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Navigation;

namespace Nyaavigator.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigationService;

    public SettingsViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private void GoSearch()
    {
        _navigationService.NavigateTo<SearchViewModel>();
    }
}