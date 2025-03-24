using Daric.Domain.Accounts;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;

namespace Daric.Application.DomainEventHandlers.Account
{
    internal class AccountCreatePersistedEventHandler(IAccountRepository accountRepository) : DomainEventHandler<AccountCreatePersisted>
    {
        public override async Task<ErrorOr<bool>> HandleAsync(AccountCreatePersisted @event, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: add bonus
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
