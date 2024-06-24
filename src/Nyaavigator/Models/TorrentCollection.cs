using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Nyaavigator.Models;

public partial class TorrentCollection : SmartCollection<Torrent>
{
    private bool _isAnyTorrentDownloading;
    public bool IsAnyTorrentDownloading
    {
        get => _isAnyTorrentDownloading;
        set
        {
            if (_isAnyTorrentDownloading == value)
                return;

            _isAnyTorrentDownloading = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAnyTorrentDownloading)));
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged
    {
        add => base.PropertyChanged += value;
        remove => base.PropertyChanged -= value;
    }

    protected override void InsertItem(int index, Torrent item)
    {
        item.PropertyChanged += OnTorrentPropertyChanged;
        base.InsertItem(index, item);
    }

    public override void AddRange(IEnumerable<Torrent> range)
    {
        var enumerable = range as Torrent[] ?? range.ToArray();

        foreach (Torrent item in enumerable)
        {
            item.PropertyChanged += OnTorrentPropertyChanged;
        }

        base.AddRange(enumerable);
    }

    private void OnTorrentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Torrent.IsDownloading))
        {
            if (sender is not Torrent torrent)
                return;

            IsAnyTorrentDownloading = torrent.IsDownloading || Items.Any(t => t.IsDownloading);
        }
    }
}