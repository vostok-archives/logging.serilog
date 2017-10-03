// Copyright 2017 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using Serilog.Core;
using Vostok.Airlock;
using Vostok.Logging.Airlock;
using SerilogEvent = Serilog.Events.LogEvent;
using SerilogEventLevel = Serilog.Events.LogEventLevel;

namespace Vostok.Logging.Serilog.Sinks
{
    /// <summary>
    ///     Writes log events to the Airlock.
    /// </summary>
    public class AirlockSink : ILogEventSink
    {
        private const int MaxMessageLength = 32 * 1024;
        private const int MaxExceptionLength = 32 * 1024;

        private readonly IAirlock airlock;
        
        /// <summary>
        ///     The sink that writes log events to the Airlock.
        /// </summary>
        /// <param name="airlock">The airlock instance</param>
        public AirlockSink(IAirlock airlock)
        {
            this.airlock = airlock;
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

            airlock.Push("logs", logEventData);
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