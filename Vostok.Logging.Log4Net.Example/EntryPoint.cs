using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using Vostok.Flow;

namespace Vostok.Logging.Log4Net.Example
{
    public static class EntryPoint
    {
        public static void Main()
        {
            XmlConfigurator.Configure();

            var log = new Log4NetLog(LogManager.GetLogger("Root"))
                .WithContext()
                .WithProperties(new Dictionary<string, object> {{"SourceClass", typeof(EntryPoint)}});

            using (Context.Properties.Use("TraceId", Guid.NewGuid()))
            {
                log.Info("Hello {Username}!", "Mike");
            }
        }
    }
}