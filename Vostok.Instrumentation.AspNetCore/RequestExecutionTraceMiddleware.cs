using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Vostok.Commons;
using Vostok.Commons.Extensions.Uri;
using Vostok.Tracing;

namespace Vostok.Instrumentation.AspNetCore
{
    public class RequestExecutionTraceMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string serviceName;

        public RequestExecutionTraceMiddleware(RequestDelegate next, string serviceName)
        {
            this.next = next;
            this.serviceName = serviceName;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var spanBuilder = Trace.BeginSpan())
            {
                var url = GetUrl(context.Request);
                spanBuilder.SetAnnotation("operationName", GetOperationName(context.Request.Method, url));
                spanBuilder.SetAnnotation("kind", "http-server");
                spanBuilder.SetAnnotation("serviceName", serviceName);
                spanBuilder.SetAnnotation("host", HostnameProvider.Get());
                spanBuilder.SetAnnotation("http.url", url.ToStringWithoutQuery());
                if (context.Request.ContentLength.HasValue)
                {
                    spanBuilder.SetAnnotation("http.requestСontentLength", context.Request.ContentLength);
                }

                await next.Invoke(context).ConfigureAwait(false);

                if (context.Response.ContentLength.HasValue)
                {
                    spanBuilder.SetAnnotation("http.responseСontentLength", context.Response.ContentLength);
                }
                spanBuilder.SetAnnotation("http.code", context.Response.StatusCode);
            }
        }

        private static string GetOperationName(string httpMethod, Uri url)
        {
            var normalizedUrl = url.Normalize();

            return httpMethod + " " + normalizedUrl;
        }

        private static Uri GetUrl(HttpRequest request)
        {
            var absoluteUrl = UriHelper.BuildAbsolute(request.Scheme, request.Host, request.PathBase, request.Path, request.QueryString);
            return new Uri(absoluteUrl);
        }

}
}