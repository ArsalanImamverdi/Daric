using Daric.Domain.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Application.Internals
{
    internal static class DomainEventHandlerExtensions
    {
        public static IServiceCollection AddDomainEventHandler<TEventHandler, TEvent>(this IServiceCollection serviceCollection)
           where TEventHandler : class, IDomainEventHandler
           where TEvent : IDomainEvent
        {
            serviceCollection.AddKeyedScoped<IDomainEventHandler, TEventHandler>(typeof(TEvent).FullName!);
            return serviceCollection;
        }
    }
}
