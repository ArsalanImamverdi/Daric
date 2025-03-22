using System.Diagnostics;

using Daric.Tracing.Abstraction;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Daric.Tracing.Zipkin;

public static class ZipkinTracingExtensions
{
    public static TracingOptions AddZipkin(this TracingOptions tracingOptions, Func<ITracingConfig, ITracingConfig> config)
    {
        tracingOptions.ServiceCollection.AddSingleton(sp => new ActivitySource(tracingOptions.Name!, tracingOptions.Version));
        tracingOptions.ServiceCollection.Configure<ZipkinConfigContainer>(tracingOptions.ServiceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("Config:Tracing"));
        tracingOptions.ServiceCollection.AddScoped<IZipkinConfig>(serviceProvider => serviceProvider.GetRequiredService<IOptionsSnapshot<ZipkinConfigContainer>>().Value?.ZipkinConfig!);

        tracingOptions.ServiceCollection.AddOpenTelemetry()
                                        .WithTracing(x =>
                                        {
                                            var traceConfig = tracingOptions.ServiceCollection.BuildServiceProvider().CreateScope().ServiceProvider.GetService<IZipkinConfig>();

                                            x.SetSampler(new TraceIdRatioBasedSampler(traceConfig?.SamplingRate ?? 1));

                                            if (traceConfig is not null)
                                                x.AddZipkinExporter(otlp =>
                                                {
                                                    otlp.Endpoint = new Uri(traceConfig.Endpoint);
                                                    otlp.ExportProcessorType = traceConfig.ExportProcessorType;
                                                    otlp.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>()
                                                    {
                                                        MaxExportBatchSize = traceConfig.MaxExportBatchSize
                                                    };
                                                });

                                            x.AddSource(tracingOptions.Name!)
                                             .ConfigureResource(res => res.AddService(tracingOptions.Name!, serviceVersion: tracingOptions.Version));


                                            Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator(
                                            [
                                                new TraceContextPropagator(),
                                                new BaggagePropagator()
                                            ]));

                                            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
                                            Activity.ForceDefaultIdFormat = true;
                                        });

        tracingOptions.ServiceCollection.AddScoped<ITrace>(sp =>
        {
            var logger = sp.GetService<ILogger<TracingOptions>>();
            var tr = sp.GetService<TracerProvider>();
            if (tr is null)
            {
                logger?.LogWarning("Can not find the {name}", nameof(TracerProvider));
                return null!;
            }

            var trr = tr.GetTracer(tracingOptions.Name ?? "Trace", tracingOptions.Version);
            var trace = new ZipkinTrace(trr);
            trace.StartRootSpan(tracingOptions.Name!);
            return trace;
        });

        config?.Invoke(new ZipkinTracingConfig());

        return tracingOptions;

    }
    public static TracingOptions AddZipkin(this TracingOptions tracingOptions)
    {
        return tracingOptions.AddZipkin(cfg => cfg);
    }

    internal class ZipkinTracingConfig : ITracingConfig
    {
    }
}
