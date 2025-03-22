using OpenTelemetry;

namespace Daric.Tracing.Zipkin;

public interface IZipkinConfig
{
    string Endpoint { get; set; }
    ExportProcessorType ExportProcessorType { get; set; }
    int MaxExportBatchSize { get; set; }
    double SamplingRate { get; set; }
}
internal class ZipkinConfig : IZipkinConfig
{
    public string Endpoint { get; set; } = string.Empty;
    public ExportProcessorType ExportProcessorType { get; set; } = ExportProcessorType.Batch;
    public int MaxExportBatchSize { get; set; } = 512;
    public double SamplingRate { get; set; } = 0.1;

}
internal class ZipkinConfigContainer
{
    public ZipkinConfig? ZipkinConfig { get; set; }
}
