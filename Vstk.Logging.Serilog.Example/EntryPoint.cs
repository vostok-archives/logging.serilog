using System;
using System.Threading;
using Serilog;
using Vstk.Flow;
using Vstk.Logging.Logs;
using Vstk.Logging.Serilog.Enrichers;

namespace Vstk.Logging.Serilog.Example
{
    public static class EntryPoint
    {
        public static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With<FlowContextEnricher>()
                //.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level} {Message:l} {Exception}{NewLine}{Properties}{NewLine}")
                .WriteTo.VstkLog(new ConsoleLog())
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

                Thread.Sleep(10000);
            }
        }
    }
}