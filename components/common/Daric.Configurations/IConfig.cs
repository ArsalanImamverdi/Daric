namespace Daric.Configurations;

public interface IConfig
{
    /// <summary>
    /// Server Listening Port
    /// </summary>
    public List<string> ServerPorts { get; set; }
    /// <summary>
    /// Code to identify a microservice
    /// </summary>
    public string? Code { get; set; }
}
