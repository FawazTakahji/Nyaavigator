namespace Nyaavigator.Models;

public class Filter(string title, int id)
{
    public string Title { get; } = title;
    public int Id { get; } = id;
}