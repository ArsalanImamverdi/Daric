using Daric.Application.DomainEventHandlers.Account;
using Daric.Application.DomainEventHandlers.Bonus;
using Daric.Application.DomainServices.Account;
using Daric.Application.EventDispatcher;
using Daric.Domain.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            serviceCollection.AddAccountDomainEventHandlers();
            serviceCollection.AddAccountDomainServices();

            serviceCollection.AddBonusDomainEventHandlers();

            return serviceCollection;
        }
    }
}
