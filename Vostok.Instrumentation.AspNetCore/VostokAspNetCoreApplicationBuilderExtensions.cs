using Microsoft.AspNetCore.Builder;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class VostokAspNetCoreApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder app)
        {
            return app.UseMiddleware<VostokAspNetCoreMiddleware>();
        }
    }
}