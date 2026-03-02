namespace Nyaavigator.Core.Navigation;

public interface INavigable
{
    public string Title { get; }
    public void OnNavigatedFrom();
}