using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Vostok.Logging;

namespace Vostok.Instrumentation.AspNetCore
{
    public class VostokAspNetCoreMiddleware : IMiddleware
    {
        private readonly ILog log;

        public VostokAspNetCoreMiddleware(VostokAspNetCoreOptions options)
        {
            log = options.Log;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            log.Info($"Start request {context.Request.Method} {context.Request.GetDisplayUrl()}");
            var stopwatch = Stopwatch.StartNew();
            await next.Invoke(context).ConfigureAwait(false);
            stopwatch.Stop();
            log.Info($"End request ({stopwatch.ElapsedMilliseconds} ms) {context.Response.StatusCode} {context.Request.Method} {context.Request.GetDisplayUrl()}");
        }
    }
}