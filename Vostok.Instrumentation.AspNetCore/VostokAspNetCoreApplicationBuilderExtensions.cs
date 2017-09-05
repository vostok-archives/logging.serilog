using Microsoft.AspNetCore.Builder;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class VostokAspNetCoreApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder builder, ILog log)
        {
            builder.UseMiddleware<VostokAspNetCoreMiddleware>(new VostokAspNetCoreOptions { Log = log });
            return builder;
        }
    }
}