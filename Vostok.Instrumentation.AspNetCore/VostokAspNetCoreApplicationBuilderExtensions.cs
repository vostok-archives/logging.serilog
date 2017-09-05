using Microsoft.AspNetCore.Builder;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class VostokAspNetCoreApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder builder, ILog log)
        {
            var middleware = new VostokAspNetCoreMiddleware(log);
            builder.Use(middleware.InvokeAsync);
            return builder;
        }
    }
}