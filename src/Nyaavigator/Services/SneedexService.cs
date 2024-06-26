using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nyaavigator.Models;
using Nyaavigator.Utilities;

namespace Nyaavigator.Services;

public class SneedexService
{
    private readonly List<SneedexEntry> _entries = [];
    private DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;

    private async Task RefreshIds()
    {
        List<SneedexEntry> entries = await Sneedex.GetIds();

        if (entries.Count > 0)
        {
            _entries.Clear();
            _entries.AddRange(entries);
            _lastUpdate = DateTimeOffset.Now;
        }
        else if (entries.Count == 0 && _entries.Count > 0)  // Avoid spamming the API
        {
            _lastUpdate = DateTimeOffset.Now;
        }
    }

    public async Task<bool> IsBestRelease(int id)
    {
        if (DateTimeOffset.Now - _lastUpdate > TimeSpan.FromHours(1))
            await RefreshIds();

        return _entries.Any(e => e.Ids.Contains(id));
    }

    public string? GetEntryId(int nyaaId)
    {
        return _entries.FirstOrDefault(e => e.Ids.Contains(nyaaId))?.EntryId;
    }
}