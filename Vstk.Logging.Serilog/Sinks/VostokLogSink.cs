using System;
using Serilog.Core;
using Serilog.Events;

namespace Vstk.Logging.Serilog.Sinks
{
    public class VstkLogSink : ILogEventSink
    {
        private readonly ILog log;

        public VstkLogSink(ILog log)
        {
            this.log = log;
        }

        public void Emit(global::Serilog.Events.LogEvent logEvent)
        {
            var vstkLogLevel = TranslateLevel(logEvent.Level);
            if (!log.IsEnabledFor(vstkLogLevel))
                return;
            var vstkLogEvent = new LogEvent(
                vstkLogLevel, 
                logEvent.Exception, 
                logEvent.MessageTemplate.Render(logEvent.Properties),
                new object[0]);
            foreach (var kvp in logEvent.Properties)
                vstkLogEvent.AddPropertyIfAbsent(kvp.Key, kvp.Value);
            log.Log(vstkLogEvent);
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