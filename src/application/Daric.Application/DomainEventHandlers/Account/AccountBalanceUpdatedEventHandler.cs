using Daric.Caching.Abstractions;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;

namespace Daric.Application.DomainEventHandlers.Account
{
    internal class AccountBalanceUpdatedEventHandler(IDistributedCacheDatabase redisDatabase) : DomainEventHandler<AccountBalanceUpdated>
    {
        public override async Task<ErrorOr<bool>> HandleAsync(AccountBalanceUpdated @event, CancellationToken cancellationToken)
        {
            try
            {
                await redisDatabase.SecureSetAsync($"account:balance:{@event.Account.AccountNumber}", @event.Account.Balance, cancellationToken);
                @event.Complete();
                return true;
            }
            catch (Exception ex)
            {
                @event.Fail(ex);
                return ex;
            }
        }
    }
}
