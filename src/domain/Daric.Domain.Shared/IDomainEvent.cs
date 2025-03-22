namespace Daric.Domain.Shared
{
    public enum ExecutionStatusType
    {
        Completed,
        Failed,
        Initialized
    }
    public interface IDomainEvent
    {
        public ExecutionStatusType ExecutionStatus { get; }
        public ErrorOr<IDomainEventResult>? Result { get; }
        public void Complete();
        public void Fail();
    }
    public record DomainEvent : DomainEvent<IDomainEventResult>
    {
    }
    public record DomainEvent<TResult> : IDomainEvent where TResult : IDomainEventResult
    {
        public ExecutionStatusType ExecutionStatus { get; private set; } = ExecutionStatusType.Initialized;

        public ErrorOr<IDomainEventResult>? Result { get; private set; }

        public ErrorOr<TResult> GetResult() => (Result as ErrorOr<TResult>)!;

        public void Complete()
        {
            ExecutionStatus = ExecutionStatusType.Completed;
        }
        public void Complete(TResult domainEventResult)
        {
            Result = new ErrorOr<IDomainEventResult>(domainEventResult);
            Complete();
        }
        public void Fail()
        {
            ExecutionStatus = ExecutionStatusType.Failed;
        }
        public void Fail(Error[] errors)
        {
            Result = new ErrorOr<IDomainEventResult>(errors);
            Fail();
        }
        public void Fail(Exception ex)
        {
            Result = new ErrorOr<IDomainEventResult>(ex);
            Fail();
        }
    }
}
