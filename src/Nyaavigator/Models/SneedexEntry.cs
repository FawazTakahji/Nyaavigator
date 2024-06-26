using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nyaavigator.Models;

public class SneedexEntry
{
    [JsonPropertyName("nyaaIDs")]
    public List<int> Ids { get; set; } = [];
    [JsonPropertyName("entryID")]
    public string EntryId { get; set; } = string.Empty;
}