using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging;
public class UtcTimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var utcTimestamp = logEvent.Timestamp.ToUniversalTime();
        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("UtcTimestamp", utcTimestamp));
    }
}
