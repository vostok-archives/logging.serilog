using Microsoft.AspNetCore.Builder;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder app, string serviceName)
        {
            return app.UseMiddleware<RequestExecutionTraceMiddleware>(serviceName)
                .UseMiddleware<RequestExecutionDistributedContextMiddleware>()
                .UseMiddleware<RequestExecutionTimeMiddleware>();
        }
    }
}