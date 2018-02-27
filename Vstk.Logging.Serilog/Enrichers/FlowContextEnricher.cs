using Serilog.Core;
using Vstk.Flow;

namespace Vstk.Logging.Serilog.Enrichers
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