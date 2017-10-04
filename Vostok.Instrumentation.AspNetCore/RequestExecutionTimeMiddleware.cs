using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public class RequestExecutionTimeMiddleware
    {
        private readonly ILog log;

        public RequestExecutionTimeMiddleware(ILog log)
        {
            this.log = log;
        }

        public async Task InvokeAsync(HttpContext context, Func<Task> next)
        {
            log.Info(
                "Request {Method} {Scheme}://{Host}{PathBase}{Path}{QueryString} started",
                context.Request.Method,
                context.Request.Scheme,
                context.Request.Host.Value,
                context.Request.PathBase.Value,
                context.Request.Path.Value,
                context.Request.QueryString.Value);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next.Invoke().ConfigureAwait(false);
            }
            finally
            {
                log.Info(
                    "Request {Method} {Scheme}://{Host}{PathBase}{Path}{QueryString} finished with {StatusCode} after {ElapsedMs} ms",
                    context.Request.Method,
                    context.Request.Scheme,
                    context.Request.Host.Value,
                    context.Request.PathBase.Value,
                    context.Request.Path.Value,
                    context.Request.QueryString.Value,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}