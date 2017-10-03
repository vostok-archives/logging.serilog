using System;
using Serilog.Core;
using SerilogEvent = Serilog.Events.LogEvent;

namespace Vostok.Logging.Serilog.Enrichers
{
    public class HostEnricher : ILogEventEnricher
    {
        private readonly string host;

        public HostEnricher()
        {
            try
            {
                host = Environment.MachineName;
            }
            catch
            {
                host = string.Empty;
            }
        }

        public void Enrich(SerilogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Host", host));
        }
    }
}