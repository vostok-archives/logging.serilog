using System;
using System.Diagnostics;
using Serilog.Core;
using SerilogEvent = Serilog.Events.LogEvent;

namespace Vostok.Logging.Serilog.Enrichers
{
    public class ServiceNameEnricher : ILogEventEnricher
    {
        private readonly string serviceName;

        public ServiceNameEnricher()
        {
            try
            {
                serviceName = Process.GetCurrentProcess().ProcessName;
            }
            catch
            {
                serviceName = string.Empty;
            }
        }

        public void Enrich(SerilogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ServiceName", serviceName));
        }
    }
}