using Topshelf.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.Topshelf
{
    public class VostokLoggingHostLoggerConfigurator : HostLoggerConfigurator
    {
        private readonly ILog log;

        public VostokLoggingHostLoggerConfigurator(ILog log)
        {
            this.log = log;
        }

        public LogWriterFactory CreateLogWriterFactory()
        {
            return new VostokLoggingLogWriterFactory(log);
        }
    }
}