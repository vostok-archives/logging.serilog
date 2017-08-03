using Microsoft.Owin.Logging;
using Owin;
using Vostok.Logging;

namespace Vostok.Instrumentation.WebApi
{
    public static class VostokOwinAppBuilderExtensions
    {
        public static IAppBuilder UseVostokOwinLogging(this IAppBuilder app, ILog log)
        {
            app.SetLoggerFactory(new VostokOwinLoggerFactory(log));
            return app;
        }

        public static IAppBuilder UseVostok(this IAppBuilder app, ILog log)
        {
            app.Use<VostokOwinMiddleware>(new VostokOwinOptions { Log = log });
            return app;
        }
    }
}