using System;
using System.Diagnostics;
using Topshelf.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.Topshelf
{
    public class VostokLoggingLogWriterAdapter : LogWriter
    {
        private readonly ILog log;

        public VostokLoggingLogWriterAdapter(ILog log)
        {
            this.log = log;
        }

        public bool IsDebugEnabled => log.IsEnabledFor(LogLevel.Debug);
        public bool IsInfoEnabled => log.IsEnabledFor(LogLevel.Info);
        public bool IsWarnEnabled => log.IsEnabledFor(LogLevel.Warn);
        public bool IsErrorEnabled => log.IsEnabledFor(LogLevel.Error);
        public bool IsFatalEnabled => log.IsEnabledFor(LogLevel.Fatal);

        public void Log(LoggingLevel level, object obj)
        {
            if (!TranslateLogLevel(level.TraceEventType, out LogLevel logLevel) || !log.IsEnabledFor(logLevel))
                return;
            log.Log(new LogEvent(logLevel, null, Convert.ToString(obj), Array.Empty<object>()));
        }

        public void Log(LoggingLevel level, object obj, Exception exception)
        {
            if (!TranslateLogLevel(level.TraceEventType, out LogLevel logLevel) || !log.IsEnabledFor(logLevel))
                return;
            log.Log(new LogEvent(logLevel, exception, Convert.ToString(obj), Array.Empty<object>()));
        }

        public void Log(LoggingLevel level, LogWriterOutputProvider messageProvider)
        {
            if (!TranslateLogLevel(level.TraceEventType, out LogLevel logLevel) || !log.IsEnabledFor(logLevel))
                return;
            log.Log(new LogEvent(logLevel, null, Convert.ToString(messageProvider()), Array.Empty<object>()));
        }

        public void LogFormat(LoggingLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
            if (!TranslateLogLevel(level.TraceEventType, out LogLevel logLevel) || !log.IsEnabledFor(logLevel))
                return;
            log.Log(new LogEvent(logLevel, null, format, args));
        }

        public void LogFormat(LoggingLevel level, string format, params object[] args)
        {
            if (!TranslateLogLevel(level.TraceEventType, out LogLevel logLevel) || !log.IsEnabledFor(logLevel))
                return;
            log.Log(new LogEvent(logLevel, null, format, args));
        }

        public void Debug(object obj)
        {
            Log(LoggingLevel.Debug, obj);
        }

        public void Debug(object obj, Exception exception)
        {
            Log(LoggingLevel.Debug, obj, exception);
        }

        public void Debug(LogWriterOutputProvider messageProvider)
        {
            Log(LoggingLevel.Debug, messageProvider);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            LogFormat(LoggingLevel.Debug, formatProvider, format, args);
        }

        public void DebugFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Debug, format, args);
        }

        public void Info(object obj)
        {
            Log(LoggingLevel.Info, obj);
        }

        public void Info(object obj, Exception exception)
        {
            Log(LoggingLevel.Info, obj, exception);
        }

        public void Info(LogWriterOutputProvider messageProvider)
        {
            Log(LoggingLevel.Info, messageProvider);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            LogFormat(LoggingLevel.Info, formatProvider, format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Info, format, args);
        }

        public void Warn(object obj)
        {
            Log(LoggingLevel.Warn, obj);
        }

        public void Warn(object obj, Exception exception)
        {
            Log(LoggingLevel.Warn, obj, exception);
        }

        public void Warn(LogWriterOutputProvider messageProvider)
        {
            Log(LoggingLevel.Warn, messageProvider);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            LogFormat(LoggingLevel.Warn, formatProvider, format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Warn, format, args);
        }

        public void Error(object obj)
        {
            Log(LoggingLevel.Error, obj);
        }

        public void Error(object obj, Exception exception)
        {
            Log(LoggingLevel.Error, obj, exception);
        }

        public void Error(LogWriterOutputProvider messageProvider)
        {
            Log(LoggingLevel.Error, messageProvider);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            LogFormat(LoggingLevel.Error, formatProvider, format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Error, format, args);
        }

        public void Fatal(object obj)
        {
            Log(LoggingLevel.Fatal, obj);
        }

        public void Fatal(object obj, Exception exception)
        {
            Log(LoggingLevel.Fatal, obj, exception);
        }

        public void Fatal(LogWriterOutputProvider messageProvider)
        {
            Log(LoggingLevel.Fatal, messageProvider);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            LogFormat(LoggingLevel.Fatal, formatProvider, format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            LogFormat(LoggingLevel.Fatal, format, args);
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