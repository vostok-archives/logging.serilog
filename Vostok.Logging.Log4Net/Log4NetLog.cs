using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Flow;
using Log4NetLevel = log4net.Core.Level;
using Log4NetEvent = log4net.Core.LoggingEvent;

namespace Vostok.Logging.Log4Net
{
    public sealed class Log4NetLog : ILog
    {
        private readonly log4net.ILog log;

        public Log4NetLog(log4net.ILog log)
        {
            this.log = log;
        }

        public void Log(LogEvent logEvent)
        {
            var level = TranslateLevel(logEvent.Level);
            if (!log.Logger.IsEnabledFor(level))
            {
                return;
            }

            IEnumerable<string> matches;
            var message = LogMessageFormatter.FormatStructuredMessage(logEvent.MessageTemplate, logEvent.MessageParameters, out matches);

            var loggingEvent = new Log4NetEvent(
                log.GetType(),
                log.Logger.Repository,
                log.Logger.Name,
                level,
                message,
                logEvent.Exception
            );

            PopulateProperties(loggingEvent, matches, logEvent.MessageParameters);
            PopulateProperties(loggingEvent, logEvent.Properties);
            PopulateProperties(loggingEvent, Context.Properties.Current);

            log.Logger.Log(loggingEvent);
        }

        public bool IsEnabledFor(LogLevel level)
        {
            return log.Logger.IsEnabledFor(TranslateLevel(level));
        }

        private static Log4NetLevel TranslateLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return Log4NetLevel.Trace;
                case LogLevel.Debug:
                    return Log4NetLevel.Debug;
                case LogLevel.Info:
                    return Log4NetLevel.Info;
                case LogLevel.Warn:
                    return Log4NetLevel.Warn;
                case LogLevel.Error:
                    return Log4NetLevel.Error;
                case LogLevel.Fatal:
                    return Log4NetLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private static void PopulateProperties(Log4NetEvent loggingEvent, IEnumerable<string> patternMatches, object[] formatParameters)
        {
            foreach (var pair in patternMatches.Zip(formatParameters, (key, value) => new { Key = key, Value = value }))
            {
                loggingEvent.Properties[pair.Key] = pair.Value;
            }
        }

        private static void PopulateProperties(Log4NetEvent loggingEvent, IReadOnlyDictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                loggingEvent.Properties[property.Key] = property.Value;
            }
        }
    }
}