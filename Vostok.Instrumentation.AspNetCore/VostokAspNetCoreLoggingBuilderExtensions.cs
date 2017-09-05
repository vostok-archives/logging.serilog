using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class VostokAspNetCoreLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddVostok(this ILoggingBuilder builder, ILog log)
        {
            var loggerProvider = new VostokAspNetCoreLoggerProvider(log);
            builder.Services.AddSingleton<ILoggerProvider>(loggerProvider);
            return builder;
        }
    }
}