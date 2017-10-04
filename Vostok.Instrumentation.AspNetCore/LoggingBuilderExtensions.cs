using System;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class LoggingBuilderExtensions
    {
        public static Microsoft.Extensions.Logging.ILoggingBuilder AddVostok(this Microsoft.Extensions.Logging.ILoggingBuilder builder, ILog log)
        {
            return Microsoft.Extensions.Logging.LoggingBuilderExtensions.AddProvider(builder, new LoggerProvider(log));
        }

        private class LoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
        {
            private readonly ILog log;

            public LoggerProvider(ILog log)
            {
                this.log = log;
            }

            public void Dispose()
            {
            }

            public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
            {
                if (string.IsNullOrEmpty(categoryName))
                {
                    return new LoggerAdapter(log);
                }

                var categoryLog = log.ForContext("aspnetcore.name", categoryName);

                if (categoryName.StartsWith("Microsoft") || categoryName.StartsWith("System"))
                {
                    return new LoggerAdapter(categoryLog.FilterByLevel(LogLevel.Error));
                }

                return new LoggerAdapter(categoryLog);
            }
        }

        private class LoggerAdapter : Microsoft.Extensions.Logging.ILogger
        {
            private readonly ILog log;

            public LoggerAdapter(ILog log)
            {
                this.log = log;
            }

            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!TranslateLogLevel(logLevel, out var vostokLogLevel) || !log.IsEnabledFor(vostokLogLevel))
                    return;

                log.Log(new LogEvent(vostokLogLevel, exception, formatter(state, exception), Array.Empty<object>()));
            }

            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                return TranslateLogLevel(logLevel, out var vostokLogLevel) && log.IsEnabledFor(vostokLogLevel);
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                // TODO: Add implementation of scope
                return new Scope();
            }

            private static bool TranslateLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel, out LogLevel vostokLogLevel)
            {
                switch (logLevel)
                {
                    case Microsoft.Extensions.Logging.LogLevel.Trace:
                        vostokLogLevel = LogLevel.Trace;
                        return true;
                    case Microsoft.Extensions.Logging.LogLevel.Debug:
                        vostokLogLevel = LogLevel.Debug;
                        return true;
                    case Microsoft.Extensions.Logging.LogLevel.Information:
                        vostokLogLevel = LogLevel.Info;
                        return true;
                    case Microsoft.Extensions.Logging.LogLevel.Warning:
                        vostokLogLevel = LogLevel.Warn;
                        return true;
                    case Microsoft.Extensions.Logging.LogLevel.Error:
                        vostokLogLevel = LogLevel.Error;
                        return true;
                    case Microsoft.Extensions.Logging.LogLevel.Critical:
                        vostokLogLevel = LogLevel.Fatal;
                        return true;
                    default:
                        vostokLogLevel = default(LogLevel);
                        return false;
                }
            }

            private class Scope : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}