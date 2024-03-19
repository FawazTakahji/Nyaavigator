using System.Collections.ObjectModel;

namespace Nyaavigator.Models;

public class Category
{
    public string Title { get; }
    public string SelectedTitle { get; }
    public string Id { get; }
    public ObservableCollection<Category>? SubCategories { get; }

    public Category(string title, string id, ObservableCollection<Category> subCategories)
    {
        Title = title;
        SelectedTitle = Title;
        Id = id;
        SubCategories = subCategories;
    }

    public Category(string title, string selectedTitle, string id)
    {
        Title = title;
        SelectedTitle = selectedTitle;
        Id = id;
    }

    public Category(string title, string id)
    {
        Title = title;
        SelectedTitle = Title;
        Id = id;
    }
}