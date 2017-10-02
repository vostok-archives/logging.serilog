using System;
using System.Diagnostics;
using Microsoft.Owin.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.WebApi
{
    public class VostokOwinLoggerAdapter : ILogger
    {
        private readonly ILog log;

        public VostokOwinLoggerAdapter(ILog log)
        {
            this.log = log;
        }

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (!TranslateLogLevel(eventType, out LogLevel logLevel) || !log.IsEnabledFor(logLevel))
                return false;

            log.Log(new LogEvent(logLevel, exception, formatter(state, exception), Array.Empty<object>()));
            return true;
        }

        private static bool TranslateLogLevel(TraceEventType eventType, out LogLevel logLevel)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    logLevel = LogLevel.Fatal;
                    return true;
                case TraceEventType.Error:
                    logLevel = LogLevel.Error;
                    return true;
                case TraceEventType.Warning:
                    logLevel = LogLevel.Warn;
                    return true;
                case TraceEventType.Information:
                    logLevel = LogLevel.Info;
                    return true;
                case TraceEventType.Verbose:
                    logLevel = LogLevel.Debug;
                    return true;
                default:
                    logLevel = default(LogLevel);
                    return false;
            }
        }
    }
}