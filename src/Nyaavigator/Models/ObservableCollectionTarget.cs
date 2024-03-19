using System.Collections.ObjectModel;
using NLog;
using NLog.Targets;

namespace Nyaavigator.Models;

public class ObservableCollectionTarget : Target
{
    public ObservableCollection<LogEventInfo> Logs { get; } = [];

    protected override void Write(LogEventInfo logEvent)
    {
        Logs.Add(logEvent);
    }
}