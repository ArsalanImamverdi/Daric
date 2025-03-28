using Daric.Domain.Accounts;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Bonuses;
using Daric.Domain.Shared;

namespace Daric.Application.DomainEventHandlers.Account
{
    internal class AccountCreatePersistedEventHandler(IAccountRepository accountRepository, IBonusRepository bonusRepository) : DomainEventHandler<AccountCreatePersisted>
    {
        public override async Task<ErrorOr<bool>> HandleAsync(AccountCreatePersisted @event, CancellationToken cancellationToken)
        {
            try
            {
                var account = await accountRepository.FirstOrDefaultAsync(account => account.AccountNumber == @event.AccountNumber);
                if (account == null)
                    return new ResourceNotFoundError(nameof(Account));

                var bonus = Domain.Bonuses.Bonus.CreateWelcomePack(account.Id);
                if (!bonus)
                    return bonus.Errors;

                await bonusRepository.Insert(bonus!);
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
