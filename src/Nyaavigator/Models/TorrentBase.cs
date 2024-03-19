using System;

namespace Nyaavigator.Models;

public class TorrentBase
{
    public string? Category { get; set; }
    public string? Name { get; set; }
    public string? Size { get; set; }
    public DateTime Date { get; set; }
    public string? Seeders { get; set; }
    public string? Leechers { get; set; }
    public string? Downloads { get; set; }
}