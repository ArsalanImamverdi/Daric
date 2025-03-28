using Daric.Domain.Accounts;
using Daric.Domain.Bonuses.Events;
using Daric.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace Daric.Application.DomainEventHandlers.Bonus
{
    internal class BonusCreatePersistedEventHandler(IAccountRepository accountRepository, ILogger<BonusCreatePersistedEventHandler> logger) : DomainEventHandler<BonusCreatePersisted>
    {
        public override async Task<ErrorOr<bool>> HandleAsync(BonusCreatePersisted @event, CancellationToken cancellationToken)
        {
            try
            {
                var account = await accountRepository.FirstOrDefaultAsync(account => account.Id == @event.AccountId);
                if (account == null)
                    return new ResourceNotFoundError(nameof(Account));

                account.Deposit(@event.Amount, @event.Description, Domain.Transactions.PerformByType.System);

                await accountRepository.Update(account);

                @event.Complete();
                return true;
            }
            catch (Exception ex)
            {
                @event.Fail(ex);
                logger.LogError(ex, nameof(BonusCreatePersistedEventHandler));
                return ex;
            }
        }
    }
}
