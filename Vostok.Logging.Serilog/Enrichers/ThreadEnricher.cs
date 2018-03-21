using System.Threading;
using Serilog.Core;

namespace Vostok.Logging.Serilog.Enrichers
{
    public class ThreadEnricher : ILogEventEnricher
    {
        public void Enrich(global::Serilog.Events.LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Thread", Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString()));
        }
    }
}