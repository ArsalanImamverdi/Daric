using Daric.Application.Internals;
using Daric.Domain.Accounts.DomainEvents;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Application.DomainEventHandlers.Account
{
    internal static class AccountDomainEventHandlerExtensions
    {
        public static IServiceCollection AddAccountDomainEventHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDomainEventHandler<AccountBalanceUpdatedEventHandler, AccountBalanceUpdated>();
            serviceCollection.AddDomainEventHandler<AccountCreatePersistedEventHandler, AccountCreatePersisted>();
            serviceCollection.AddDomainEventHandler<AccountDebitCompletedEventHandler, AccountDebitCompleted>();
            serviceCollection.AddDomainEventHandler<AccountCreditCompletedEventHandler, AccountCreditCompleted>();

            return serviceCollection;
        }


    }
}
