using System;

namespace Nyaavigator.Models;

public class Comment
{
    public User User { get; init; } = new();
    public DateTime Date { get; set; }
    public DateTime EditedDate { get; set; }
    public bool IsEdited { get; set; }
    public bool IsUploader { get; set; }
    public string? Text { get; set; }
}