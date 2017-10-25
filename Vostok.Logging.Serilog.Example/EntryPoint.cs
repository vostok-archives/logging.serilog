using System;
using System.Threading;
using Serilog;
using Vostok.Flow;
using Vostok.Logging.Serilog.Enrichers;

namespace Vostok.Logging.Serilog.Example
{
    public static class EntryPoint
    {
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With<FlowContextEnricher>()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level} {Message:l} {Exception}{NewLine}{Properties}{NewLine}")
                .CreateLogger();

            var log = new SerilogLog(Log.Logger)
                .ForContext(typeof(EntryPoint));

            while (true)
            {
                using (Context.Properties.Use("TraceId", Guid.NewGuid()))
                {
                    log.Info("Hello {Username}!", "Mike");
                }
            
                log.Info("Hello Bob!");

                Thread.Sleep(1000);
            }
        }
    }
}