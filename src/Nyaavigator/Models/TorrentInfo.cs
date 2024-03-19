using System.Collections.Generic;

namespace Nyaavigator.Models;

public class TorrentInfo : TorrentBase
{
    public List<Comment> Comments { get; init; } = [];
    public List<ListItem> Items { get; init; } = [];
    public User Submitter { get; init; } = new();
    public string? Description { get; set; }
    public string? Hash { get; set; }
    public string? Information { get; set; }
}