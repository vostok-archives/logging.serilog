using Microsoft.Owin.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.WebApi
{
    public class VostokOwinLoggerFactory : ILoggerFactory
    {
        private readonly ILog log;

        public VostokOwinLoggerFactory(ILog log)
        {
            this.log = log;
        }

        public ILogger Create(string name)
        {
            return new VostokOwinLoggerAdapter(string.IsNullOrEmpty(name) ? log : log.ForContext("owin.name", name));
        }
    }
}