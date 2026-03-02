using Nyaavigator.Core.Navigation;

namespace Nyaavigator.Core.ViewModels;

public class NavigationViewModelBase : ViewModelBase, INavigable
{
    public virtual string Title { get; } = string.Empty;

    public virtual void OnNavigatedFrom()
    {

    }
}