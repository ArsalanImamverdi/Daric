namespace Daric.Scheduling.Abstraction
{
    public interface IJob
    {
        Task ExecuteAsync(string name, IJobArguments arguments);
    }
}
