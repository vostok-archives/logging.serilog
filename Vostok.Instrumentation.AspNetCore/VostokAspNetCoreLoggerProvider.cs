using Microsoft.Extensions.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public class VostokAspNetCoreLoggerProvider : ILoggerProvider
    {
        private readonly ILog log;

        public VostokAspNetCoreLoggerProvider(ILog log)
        {
            this.log = log;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new VostokAspNetCoreLoggerAdapter(string.IsNullOrEmpty(categoryName) ? log : log.ForContext("aspnetcore.name", categoryName));
        }
    }
}
