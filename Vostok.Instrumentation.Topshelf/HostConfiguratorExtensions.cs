using Topshelf.HostConfigurators;
using Topshelf.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.Topshelf
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseVostokLogging(this HostConfigurator configurator, ILog log)
        {
            HostLogger.UseLogger(new VostokLoggingHostLoggerConfigurator(log));
            return configurator;
        }
    }
}