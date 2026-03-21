using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nyaavigator.Core.Navigation;
using Nyaavigator.Core.Nyaa;
using SearchOption = Nyaavigator.Core.Nyaa.SearchOption;

namespace Nyaavigator.Core.ViewModels;

public partial class SearchViewModel : ViewModelBase, INavigable
{
    private readonly NavigationService _navigationService;

    public List<Category> Categories { get; }
    public List<SearchOption> Filters { get; }
    public List<SearchOption> SortOptions { get; }
    public List<SearchOption> SortOrders { get; }

    [ObservableProperty]
    public partial string Username { get; set; }

    [ObservableProperty]
    public partial Category SelectedCategory { get; set; }

    [ObservableProperty]
    public partial SearchOption SelectedFilter { get; set; }

    [ObservableProperty]
    public partial SearchOption SelectedSortOption { get; set; }

    [ObservableProperty]
    public partial SearchOption SelectedSortOrder { get; set; }

    public SearchViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;

        Categories = Category.GetNyaaCategories();
        Filters = GetFilters();
        SortOptions = GetSortOptions();
        SortOrders = GetSortOrders();

        Username = string.Empty;
        SelectedCategory = Categories[0];
        SelectedFilter = Filters[0];
        SelectedSortOption = SortOptions[0];
        SelectedSortOrder = SortOrders[0];
    }

    [RelayCommand]
    private void PushSettings()
    {
        _navigationService.Push<SettingsViewModel>();
    }

    [RelayCommand]
    private void SetSelectedCategory(Category? category)
    {
        if (category is not null)
        {
            SelectedCategory = category;
        }
    }

    [RelayCommand]
    private void SetSelectedFilter(SearchOption filter)
    {
        SelectedFilter = filter;
    }

    [RelayCommand]
    private void SetSelectedSortOption(SearchOption sortOption)
    {
        SelectedSortOption = sortOption;
    }

    [RelayCommand]
    private void SetSelectedSortOrder(SearchOption sortOrder)
    {
        SelectedSortOrder = sortOrder;
    }

    [RelayCommand]
    private void ResetOptions()
    {
        Username = string.Empty;
        SelectedCategory = Categories[0];
        SelectedFilter = Filters[0];
        SelectedSortOption = SortOptions[0];
        SelectedSortOrder = SortOrders[0];
    }

    private static List<SearchOption> GetFilters()
    {
        return
        [
            new("No Filter", "0"),
            new("No Remakes", "1"),
            new("Trusted Only", "2"),
        ];
    }

    private static List<SearchOption> GetSortOptions()
    {
        return
        [
            new("Date", "date"),
            new("Seeders", "seeders"),
            new("Leechers", "leechers"),
            new("Downloads", "downloads"),
            new("Size", "size"),
            new("Comments", "comments")
        ];
    }

    private static List<SearchOption> GetSortOrders()
    {
        return
        [
            new("Ascending", "asc"),
            new("Descending", "desc"),
        ];
    }
}