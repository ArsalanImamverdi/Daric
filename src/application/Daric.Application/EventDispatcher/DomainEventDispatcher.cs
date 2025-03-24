using Daric.Domain.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Daric.Application.EventDispatcher
{
    internal class DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
    {
        public async Task<ErrorOr<bool>> DispatchAllAsync(List<IDomainEvent> domainEvents)
        {
            var tasks = domainEvents.Select(DispatchAsync);
            var result = await Task.WhenAll(tasks);
            if (result.All(res => res.Success))
                return true;

            return result.SelectMany(res => res.Errors).ToList();
        }

        public async Task<ErrorOr<bool>> DispatchAsync(IDomainEvent domainEvent)
        {
            if (domainEvent is null)
                return true;
            try
            {

                var domainEventName = domainEvent.GetType().FullName ?? domainEvent.GetType().Name;

                logger.LogTrace("Start to execute DomainEvent {name}", domainEventName);

                var domainEventHandler = serviceProvider.GetKeyedService<IDomainEventHandler>(domainEventName);
                if (domainEventHandler is null)
                {
                    domainEvent.Complete();
                    logger.LogWarning("Can not find DomainEventHandler for {domainEvent}", domainEventName);
                    return true;
                }

                var result = await domainEventHandler.HandleAsync(domainEvent, CancellationToken.None);
                if (result is null || !result.Success)
                {
                    logger.LogWarning("Execution of DomainEventHandler for {name} with {errors}", domainEventName, result!.Errors.Select(err => err.Message).Aggregate((f, s) => $"{f},{s}"));
                    return result.Errors;
                }

                logger.LogTrace("DomainEvent {name} executed successfully", domainEventName);

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(DomainEventDispatcher));
                return ex;
            }
        }
    }
}
