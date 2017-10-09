using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Metrics;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVostok(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            var service = configuration.GetValue<string>("service");

            return app.UseMiddleware<RequestExecutionTimeMiddleware>(service)
                .UseMiddleware<RequestExecutionDistributedContextMiddleware>()
                .UseMiddleware<RequestExecutionTraceMiddleware>()
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