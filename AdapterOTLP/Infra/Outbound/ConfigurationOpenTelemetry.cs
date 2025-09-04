using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace Infra.Configuration.Outbound
{
    public static class ConfigurationOpenTelemetry
    {
        public static IServiceCollection AddOpenTelemetryAdapter(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            var telemetrySection = configuration.GetSection("OpenTelemetry_Settings");
            var endpointUrl = Environment.GetEnvironmentVariable("OTLP_ENDPOINT") ?? telemetrySection["Endpoint"];
            var serviceName = Environment.GetEnvironmentVariable("OTLP_SERVICE_NAME") ?? telemetrySection["Service"] ?? "adapter-otlp";

            if (string.IsNullOrWhiteSpace(endpointUrl))
                throw new InvalidOperationException("OTLP endpoint is required. Set OTLP_ENDPOINT environment variable or configure OpenTelemetry_Settings:Endpoint");

            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0");

            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(endpointUrl);
                            // options.TimeoutMilliseconds = 30000;
                        })
                        .AddConsoleExporter()
                        .AddSource(Assembly.GetExecutingAssembly().GetName().Name!)
                        .AddSource(serviceName)
                        .SetResourceBuilder(resourceBuilder)
                        .SetSampler(new TraceIdRatioBasedSampler(1.0))
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            // filtrar requests desnecessários
                            options.Filter = context =>
                                !context.Request.Path.StartsWithSegments("/health") &&
                                !context.Request.Path.StartsWithSegments("/metrics");
                        });
                });

            return services;
        }
    }
}
