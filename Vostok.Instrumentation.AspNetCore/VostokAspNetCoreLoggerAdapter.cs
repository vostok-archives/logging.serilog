using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Vostok.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Vostok.Instrumentation.AspNetCore
{
    public class VostokAspNetCoreLoggerAdapter : ILogger
    {
        private readonly ILog log;

        public VostokAspNetCoreLoggerAdapter(ILog log)
        {
            this.log = log;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!TranslateLogLevel(logLevel, out var vostokLogLevel) || !log.IsEnabledFor(vostokLogLevel))
                return;
            log.Log(new LogEvent(vostokLogLevel, exception, formatter(state, exception), Array.Empty<object>(), new Dictionary<string, object>()));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return TranslateLogLevel(logLevel, out var vostokLogLevel) && log.IsEnabledFor(vostokLogLevel);
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        private static bool TranslateLogLevel(LogLevel logLevel, out Logging.LogLevel vostokLogLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    vostokLogLevel = Logging.LogLevel.Trace;
                    return true;
                case LogLevel.Debug:
                    vostokLogLevel = Logging.LogLevel.Debug;
                    return true;
                case LogLevel.Information:
                    vostokLogLevel = Logging.LogLevel.Info;
                    return true;
                case LogLevel.Warning:
                    vostokLogLevel = Logging.LogLevel.Warn;
                    return true;
                case LogLevel.Error:
                    vostokLogLevel = Logging.LogLevel.Error;
                    return true;
                case LogLevel.Critical:
                    vostokLogLevel = Logging.LogLevel.Fatal;
                    return true;
                default:
                    vostokLogLevel = default(Logging.LogLevel);
                    return false;
            }
        }
    }
}