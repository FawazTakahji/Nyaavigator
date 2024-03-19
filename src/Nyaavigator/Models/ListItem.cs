using System.Collections.Generic;
using Material.Icons;

namespace Nyaavigator.Models;

public class ListItem
{
    public bool IsFolder { get; init; }
    public string? Name { get; init; }
    public string? Size { get; init; }
    public List<ListItem> Children { get; init; } = [];
    public MaterialIconKind Icon => IsFolder ? MaterialIconKind.Folder : MaterialIconKind.File;

    public string? Details
    {
        get
        {
            if (!IsFolder)
                return Size;

            int fileCount = GetFileCount();
            return fileCount == 1 ? "1 File" : $"{fileCount} Files";
        }
    }

    private int GetFileCount()
    {
        int count = 0;

        foreach (ListItem child in Children)
        {
            if (!child.IsFolder)
                count++;

            count += child.GetFileCount();
        }

        return count;
    }
}