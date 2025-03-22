namespace Daric.Configurations;

/// <summary>
/// Config Base Class
/// </summary>
public class Config : IConfig
{
    /// <summary>
    /// Server Listening Port
    /// </summary>
    public List<string> ServerPorts { get; set; } = [];

    /// <summary>
    /// Code to identify a microservice
    /// </summary>
    public string? Code { get; set; }
}
