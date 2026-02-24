using Nyaavigator.Core.Navigation;

namespace Nyaavigator.Core.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public NavigationService NavigationService { get; }

    public MainViewModel(NavigationService navigationService)
    {
        NavigationService = navigationService;

        NavigationService.NavigateTo<SearchViewModel>();
    }
}