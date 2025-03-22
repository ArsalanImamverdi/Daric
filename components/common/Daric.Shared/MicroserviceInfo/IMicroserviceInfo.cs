namespace Daric.Shared;

public interface IMicroserviceInfo
{
    string Name { get; }
    int ProcessId { get; }
    long Id { get; }
}
