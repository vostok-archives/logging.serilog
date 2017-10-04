using Microsoft.AspNetCore.Builder;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestExecutionTimeMiddleware>();
        }
    }
}