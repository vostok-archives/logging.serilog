using Serilog.Core;
using Vostok.Commons;
using SerilogEvent = Serilog.Events.LogEvent;

namespace Vostok.Logging.Serilog.Enrichers
{
    public class HostEnricher : ILogEventEnricher
    {
        public void Enrich(SerilogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Host", HostnameProvider.Get()));
        }
    }
}