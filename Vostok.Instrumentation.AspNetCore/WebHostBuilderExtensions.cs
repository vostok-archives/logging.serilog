using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Vostok.Airlock;
using Vostok.Clusterclient.Topology;
using Vostok.Commons.Extensions.UnitConvertions;
using Vostok.Logging;
using Vostok.Logging.Serilog;
using Vostok.Metrics;
using Vostok.Tracing;

namespace Vostok.Instrumentation.AspNetCore
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseVostok(this IWebHostBuilder builder)
        {
            return builder
                .ConfigureAirlock()
                .ConfigureVostokMetrics()
                .ConfigureVostokLogging()
                .ConfigureVostokTracing();
        }

        public static IWebHostBuilder ConfigureVostokLogging(this IWebHostBuilder builder)
        {
            return builder.ConfigureLogging((hostingContext, logging) =>
                {
                    var serviceProvider = logging.Services.BuildServiceProvider();
                    var airlockClient = serviceProvider.GetService<IAirlockClient>();
                    var log = BuildLog(hostingContext.Configuration, airlockClient);
                    logging.AddVostok(log);
                    logging.Services.AddSingleton(log);
                });
        }

        public static IWebHostBuilder ConfigureAirlock(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices((hostingContext, services) =>
            {
                var airlockSection = hostingContext.Configuration.GetSection("airlock");
                var airlockConfigSection = airlockSection.GetSection("config");
                var airlockApiKey = airlockConfigSection.GetValue<string>("apiKey");
                var airlockHost = airlockConfigSection.GetValue<Uri>("host");
                var log = BuildLog(hostingContext.Configuration, null);

                var airlockClient = new AirlockClient(new AirlockConfig
                {
                    ApiKey = airlockApiKey,
                    ClusterProvider = new FixedClusterProvider(airlockHost)
                }, log);
                services.AddSingleton<IAirlockClient>(airlockClient);
            });
        }

        public static IWebHostBuilder ConfigureVostokTracing(this IWebHostBuilder builder)
        {
            return builder
                .ConfigureServices((hostingContext, services) =>
                {
                    Trace.Configuration.IsEnabled = () => true;

                    var serviceProvider = services.BuildServiceProvider();
                    Trace.Configuration.AirlockClient = serviceProvider.GetService<IAirlockClient>();
                    
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    var service = configuration.GetValue<string>("service");
                    var project = configuration.GetValue<string>("project");
                    var environment = configuration.GetValue<string>("environment");
                    var routingKey = RoutingKey.Create(project, environment, service, "traces");
                    Trace.Configuration.AirlockRoutingKey = () => routingKey;
                });
        }

        public static IWebHostBuilder ConfigureVostokMetrics(this IWebHostBuilder builder)
        {
            return builder
                .ConfigureServices((hostingContext, services) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    var airlockClient = serviceProvider.GetService<IAirlockClient>();
                    var service = hostingContext.Configuration.GetValue<string>("service");
                    var project = hostingContext.Configuration.GetValue<string>("project");
                    var environment = hostingContext.Configuration.GetValue<string>("environment");
                    var routingKeyPrefix = RoutingKey.CreatePrefix(project, environment, service);
                    var airlockMetricReporter = new AirlockMetricReporter(airlockClient, routingKeyPrefix);
                    var metricConfiguration = new MetricConfiguration
                    {
                        Reporter = airlockMetricReporter,
                        Environment = environment
                    };
                    services.AddSingleton<IMetricConfiguration>(metricConfiguration);
                    services.AddSingleton<IMetricScope>(new RootMetricScope(metricConfiguration));
                });
        }

        private static ILog BuildLog(IConfiguration configuration, IAirlockClient airlockClient)
        {
            var loggingSection = configuration.GetSection("logging");

            var rollingFileSection = loggingSection.GetSection("rollingFile");
            var rollingFilePathFormat = rollingFileSection.GetValue<string>("pathFormat");

            var service = configuration.GetValue<string>("service");
            var project = configuration.GetValue<string>("project");
            var environment = configuration.GetValue<string>("environment");
            var routingKeyPrefix = RoutingKey.Create(project, environment, service, "logs");

            const string outputTemplate = "{Timestamp:HH:mm:ss.fff} {Level} {Message:l} {Exception}{NewLine}{Properties}{NewLine}";

            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithHost()
                .Enrich.WithProperty("Service", service);

            if (airlockClient != null)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Airlock(airlockClient, routingKeyPrefix);
            }

            Log.Logger = loggerConfiguration
                .WriteTo.Async(x => x.RollingFile(rollingFilePathFormat, outputTemplate: outputTemplate))
                .WriteTo.Console(outputTemplate: outputTemplate)
                .CreateLogger();

            return new SerilogLog(Log.Logger).WithFlowContext();
        }
    }
}