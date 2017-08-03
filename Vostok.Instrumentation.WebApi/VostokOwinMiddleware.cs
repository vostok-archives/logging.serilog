using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Vostok.Logging;

namespace Vostok.Instrumentation.WebApi
{
    public class VostokOwinMiddleware : OwinMiddleware
    {
        private readonly OwinMiddleware next;
        private readonly ILog log;

        public VostokOwinMiddleware(OwinMiddleware next, VostokOwinOptions options)
            : base(next)
        {
            this.next = next;
            log = options.Log;
        }

        public override async Task Invoke(IOwinContext context)
        {
            log.Info($"Start request {context.Request.Method} {context.Request.Uri}");
            var stopwatch = Stopwatch.StartNew();
            await next.Invoke(context).ConfigureAwait(false);
            stopwatch.Stop();
            log.Info($"End request ({stopwatch.ElapsedMilliseconds} ms) {context.Response.StatusCode} {context.Request.Method} {context.Request.Uri}");
        }
    }
}