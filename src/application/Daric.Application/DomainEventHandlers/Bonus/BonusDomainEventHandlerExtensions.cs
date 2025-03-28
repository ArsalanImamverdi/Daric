using Daric.Application.Internals;
using Daric.Domain.Bonuses.Events;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Application.DomainEventHandlers.Bonus
{
    internal static class BonusDomainEventHandlerExtensions
    {
        public static IServiceCollection AddBonusDomainEventHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDomainEventHandler<BonusCreatePersistedEventHandler, BonusCreatePersisted>();
            return serviceCollection;
        }
    }
}
