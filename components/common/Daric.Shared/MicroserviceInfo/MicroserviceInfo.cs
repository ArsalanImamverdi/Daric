namespace Daric.Shared;

public class MicroserviceInfo : IMicroserviceInfo
{
    public string Name { get; internal set; }

    public int ProcessId { get; internal set; }
    public long Id { get; internal set; }

    public MicroserviceInfo()
    {
        Name = string.Empty;
    }
}

public class MicroserviceInfoBuilder
{
    private readonly MicroserviceInfo microserviceInfo;
    public MicroserviceInfoBuilder()
    {
        microserviceInfo = new MicroserviceInfo();
    }

    public MicroserviceInfoBuilder(MicroserviceInfo microserviceInfo)
    {
        this.microserviceInfo = microserviceInfo;
    }

    public MicroserviceInfoBuilder WithName(string name)
    {
        microserviceInfo.Name = name;
        return this;
    }

    public MicroserviceInfoBuilder WithProcessId(int processId)
    {
        microserviceInfo.ProcessId = processId;
        return this;
    }

    public MicroserviceInfoBuilder WithId(long id)
    {
        microserviceInfo.Id = id;
        return this;
    }

    public MicroserviceInfo Build()
    {
        return microserviceInfo;
    }
}
