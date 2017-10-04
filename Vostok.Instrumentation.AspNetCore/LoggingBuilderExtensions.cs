using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddVostok(this ILoggingBuilder builder, Logging.ILog log)
        {
            var loggerProvider = new LoggerProvider(log);
            builder.Services.AddSingleton<ILoggerProvider>(loggerProvider);
            return builder;
        }

        private class LoggerProvider : ILoggerProvider
        {
            private readonly Logging.ILog log;

            public LoggerProvider(Logging.ILog log)
            {
                this.log = log;
            }

            public void Dispose()
            {
            }

            public ILogger CreateLogger(string categoryName)
            {
                return new LoggerAdapter(string.IsNullOrEmpty(categoryName) ? log : Logging.ILogExtensions.ForContext(log, "aspnetcore.name", categoryName));
            }
        }

        private class LoggerAdapter : ILogger
        {
            private readonly Logging.ILog log;

            public LoggerAdapter(Logging.ILog log)
            {
                this.log = log;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!TranslateLogLevel(logLevel, out var vostokLogLevel) || !log.IsEnabledFor(vostokLogLevel))
                    return;

                log.Log(new Logging.LogEvent(vostokLogLevel, exception, formatter(state, exception), Array.Empty<object>()));
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return TranslateLogLevel(logLevel, out var vostokLogLevel) && log.IsEnabledFor(vostokLogLevel);
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                // TODO: Add implementation of scope
                return new Scope();
            }

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

            private class Scope : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}