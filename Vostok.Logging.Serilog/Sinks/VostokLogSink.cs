using System;
using Serilog.Core;
using Serilog.Events;

namespace Vostok.Logging.Serilog.Sinks
{
    public class VostokLogSink : ILogEventSink
    {
        private readonly ILog log;

        public VostokLogSink(ILog log)
        {
            this.log = log;
        }

        public void Emit(global::Serilog.Events.LogEvent logEvent)
        {
            var vostokLogLevel = TranslateLevel(logEvent.Level);
            if (!log.IsEnabledFor(vostokLogLevel))
                return;
            var vostokLogEvent = new LogEvent(
                vostokLogLevel, 
                logEvent.Exception, 
                logEvent.MessageTemplate.Render(logEvent.Properties),
                new object[0]);
            foreach (var kvp in logEvent.Properties)
                vostokLogEvent.AddPropertyIfAbsent(kvp.Key, kvp.Value);
            log.Log(vostokLogEvent);
        }

        private LogLevel TranslateLevel(LogEventLevel logEventLevel)
        {
            switch (logEventLevel)
            {
                case LogEventLevel.Verbose:
                    return LogLevel.Trace;
                case LogEventLevel.Debug:
                    return LogLevel.Debug;
                case LogEventLevel.Information:
                    return LogLevel.Info;
                case LogEventLevel.Warning:
                    return LogLevel.Warn;
                case LogEventLevel.Error:
                    return LogLevel.Error;
                case LogEventLevel.Fatal:
                    return LogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logEventLevel), logEventLevel, null);
            }
        }
    }
}