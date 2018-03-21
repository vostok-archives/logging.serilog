using Serilog.Core;
using Vostok.Flow;

namespace Vostok.Logging.Serilog.Enrichers
{
    public class FlowContextEnricher : ILogEventEnricher
    {
        public void Enrich(global::Serilog.Events.LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            foreach (var kvp in Context.Properties.Current)
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(kvp.Key, kvp.Value));
        }
    }
}