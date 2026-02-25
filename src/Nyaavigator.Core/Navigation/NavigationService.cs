using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ObservableCollections;

namespace Nyaavigator.Core.Navigation;

public partial class NavigationService : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ObservableStack<INavigable> _history = new();

    [ObservableProperty]
    private INavigable? _current;
    [ObservableProperty]
    private bool _isGoingBack;
    public bool CanGoBack => _history.Count > 0;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _history.CollectionChanged += OnHistoryCollectionChanged;
    }

    private void OnHistoryCollectionChanged(in NotifyCollectionChangedEventArgs<INavigable> e)
    {
        OnPropertyChanged(nameof(CanGoBack));
        PopCommand.NotifyCanExecuteChanged();
    }

    public void NavigateTo<TNavigable>() where TNavigable : INavigable
    {
        _history.Clear();
        IsGoingBack = false;
        Current = _serviceProvider.GetRequiredService<TNavigable>();
    }

    public void Push<TNavigable>() where TNavigable : INavigable
    {
        if (Current is not null)
        {
            _history.Push(Current);
        }

        IsGoingBack = false;
        Current = _serviceProvider.GetRequiredService<TNavigable>();
    }

    public void Push(INavigable item)
    {
        if (Current is not null)
        {
            _history.Push(Current);
        }

        IsGoingBack = false;
        Current = item;
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    public void Pop()
    {
        if (_history.Count < 1)
        {
            return;
        }

        IsGoingBack = true;
        Current = _history.Pop();
    }
}