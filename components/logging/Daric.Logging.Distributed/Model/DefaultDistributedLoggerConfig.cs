using Daric.Logging.Abstractions;

namespace Daric.Logging.Distributed.Model
{
    internal class DefaultDistributedLoggerConfig : IDistributedLoggerConfig
    {
        public DistributedLoggerConfig? DistributedLoggerConfig { get; set; }
    }
}
