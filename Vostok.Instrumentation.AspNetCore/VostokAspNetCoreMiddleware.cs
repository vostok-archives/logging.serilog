using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public class VostokAspNetCoreMiddleware
    {
        private readonly ILog log;

        public VostokAspNetCoreMiddleware(ILog log)
        {
            this.log = log;
        }

        public async Task InvokeAsync(HttpContext context, Func<Task> next)
        {
            log.Info($"Start request {context.Request.Method} {context.Request.GetDisplayUrl()}");
            var stopwatch = Stopwatch.StartNew();
            await next.Invoke().ConfigureAwait(false);
            stopwatch.Stop();
            log.Info($"End request ({stopwatch.ElapsedMilliseconds} ms) {context.Response.StatusCode} {context.Request.Method} {context.Request.GetDisplayUrl()}");
        }
    }
}