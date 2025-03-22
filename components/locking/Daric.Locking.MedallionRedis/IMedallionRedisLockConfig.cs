namespace Daric.Locking.MedallionRedis
{
    public interface IMedallionRedisLockConfig
    {
        public MedallionRedisLockConfig? MedallionRedisLockConfig { get; set; }
    }

    public class MedallionRedisLockConfig
    {
        public List<ServerAddressPort> ServerAddressPorts { get; set; } = [];
        public int DatabaseIndex { get; set; }
        public string? Password { get; set; }
        public int? ConnectionTimeout { get; set; }
        public string? KeyPrefix { get; set; }
    }

    public class ServerAddressPort
    {
        public int ServerPort { get; set; } = 0;
        public string ServerAddress { get; set; } = string.Empty;
    }

    public class DefaultMedallionRedisLockConfig : IMedallionRedisLockConfig
    {
        public MedallionRedisLockConfig? MedallionRedisLockConfig { get; set; }
    }
}
