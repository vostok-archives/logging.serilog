using System;
using System.Linq;
using Serilog.Core;
using Vostok.Airlock;
using Vostok.Logging.Airlock;
using SerilogEvent = Serilog.Events.LogEvent;
using SerilogEventLevel = Serilog.Events.LogEventLevel;

namespace Vostok.Logging.Serilog.Sinks
{
    public class AirlockSink : ILogEventSink
    {
        static AirlockSink()
        {
            AirlockSerializerRegistry.Register(new LogEventDataSerializer());
        }

        private const int MaxMessageLength = 32 * 1024;
        private const int MaxExceptionLength = 32 * 1024;

        private readonly IAirlockClient airlockClient;
        private readonly string routingKey;

        public AirlockSink(IAirlockClient airlockClient, string routingKey)
        {
            this.airlockClient = airlockClient;
            this.routingKey = routingKey;
        }

        public void Emit(SerilogEvent logEvent)
        {
            var logEventData = new LogEventData
            {
                Timestamp = logEvent.Timestamp,
                Level = TranslateLevel(logEvent.Level),
                Message = logEvent.MessageTemplate.Render(logEvent.Properties).Truncate(MaxMessageLength),
                Exception = logEvent.Exception?.ToString().Truncate(MaxExceptionLength),
                Properties = logEvent.Properties.ToDictionary(x => x.Key, x => x.Value.ToString())
            };

            airlockClient.Push(routingKey, logEventData);
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