using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vostok.Hosting;

namespace Vostok.Instrumentation.AspNetCore
{
    public abstract class AspNetCoreVostokApplication : IVostokApplication
    {
        private Task workTask;

        protected IVostokHostingEnvironment HostingEnvironment { get; private set; }

        public async Task StartAsync(IVostokHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
            var webHost = BuildWebHost();
            var applicationLifetime = webHost.Services.GetRequiredService<IApplicationLifetime>();
            var tcs = new TaskCompletionSource<int>();
            applicationLifetime.ApplicationStarted.Register(() => tcs.TrySetResult(0));
            workTask = webHost.RunAsync(hostingEnvironment.ShutdownCancellationToken);
            await tcs.Task.ConfigureAwait(false);
        }

        public async Task WaitForTerminationAsync()
        {
            await workTask.ConfigureAwait(false);
        }

        protected abstract IWebHost BuildWebHost();
    }
}