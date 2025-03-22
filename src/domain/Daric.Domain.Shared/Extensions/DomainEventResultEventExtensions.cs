namespace Daric.Domain.Shared
{
    public static class DomainEventResultEventExtensions
    {
        public static TResult? OfType<TResult>(this List<IDomainEventResult> domainEvents) where TResult : class, IDomainEventResult
        {
            return domainEvents.FirstOrDefault(de => de is TResult) as TResult;
        }
    }
}
