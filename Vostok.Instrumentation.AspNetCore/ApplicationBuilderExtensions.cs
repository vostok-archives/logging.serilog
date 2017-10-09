using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Metrics;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder app, string serviceName)
        {
            return app.UseMiddleware<RequestExecutionTraceMiddleware>(serviceName)
                .UseMiddleware<RequestExecutionDistributedContextMiddleware>()
                .UseMiddleware<RequestExecutionTimeMiddleware>()
                .UseVostokLogging()
                .UseVostokSystemMetrics(10.Seconds());
        }

        public static IApplicationBuilder UseVostokLogging(this IApplicationBuilder app)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            return app;
        }

        public static IApplicationBuilder UseVostokSystemMetrics(this IApplicationBuilder app, TimeSpan period)
        {
            var metricConfiguration = app.ApplicationServices.GetService<IMetricConfiguration>();
            new RootMetricScope(metricConfiguration).SystemMetrics(period);

            return app;
        }
    }
}