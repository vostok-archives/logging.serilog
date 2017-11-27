using System;
using System.Linq;
using Serilog.Core;
using Vostok.Airlock;
using Vostok.Airlock.Logging;
using Vostok.Commons;
using Vostok.Hosting;
using SerilogEvent = Serilog.Events.LogEvent;
using SerilogEventLevel = Serilog.Events.LogEventLevel;

namespace Vostok.Logging.Serilog.Sinks
{
    public class AirlockSink : ILogEventSink
    {
        private readonly Func<IAirlockClient> getAirlockClient;
        private readonly Func<string> getRoutingKey;
        private const int maxMessageLength = 32*1024;
        private const int maxExceptionLength = 32*1024;

        public AirlockSink()
            : this(() => VostokHostingEnvironment.Current?.AirlockClient, () => VostokHostingEnvironment.Current?.GetLoggingRoutingKey())
        {

        }

        public AirlockSink(Func<IAirlockClient> getAirlockClient, Func<string> getRoutingKey)
        {
            this.getAirlockClient = getAirlockClient;
            this.getRoutingKey = getRoutingKey;
        }

        public AirlockSink(IAirlockClient airlockClient, string routingKey)
            : this(() => airlockClient, () => routingKey)
        {
        }

        public void Emit(SerilogEvent logEvent)
        {
            var airlockClient = getAirlockClient();
            var routingKey = getRoutingKey();
            if (airlockClient == null || string.IsNullOrEmpty(routingKey))
                return;
            var logEventData = new LogEventData
            {
                Timestamp = logEvent.Timestamp,
                Level = TranslateLevel(logEvent.Level),
                Message = logEvent.MessageTemplate.Render(logEvent.Properties).Truncate(maxMessageLength),
                Exception = logEvent.Exception?.ToString().Truncate(maxExceptionLength),
                Properties = logEvent.Properties.ToDictionary(x => x.Key, x => x.Value.ToString())
            };
            // todo (spaceorc, 13.10.2017) make "host" constant somewhere in Vostok.Core/LogPropertyNames.cs
            logEventData.Properties["host"] = HostnameProvider.Get();

            airlockClient.Push(routingKey, logEventData, logEventData.Timestamp);
        }

        private LogLevel TranslateLevel(SerilogEventLevel level)
        {
            switch (level)
            {
                case SerilogEventLevel.Verbose:
                    return LogLevel.Trace;
                case SerilogEventLevel.Debug:
                    return LogLevel.Debug;
                case SerilogEventLevel.Information:
                    return LogLevel.Info;
                case SerilogEventLevel.Warning:
                    return LogLevel.Warn;
                case SerilogEventLevel.Error:
                    return LogLevel.Error;
                case SerilogEventLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}