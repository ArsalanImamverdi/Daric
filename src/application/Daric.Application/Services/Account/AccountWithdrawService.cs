using Daric.Domain.Accounts;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;
using Daric.Locking.Abstraction;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Services.Account
{
    public class AccountWithdrawService(IAccountRepository accountRepository,
                                        IUnitOfWork unitOfWork,
                                        IDistributedLockMechanism distributedLockMechanism,
                                        ILogger<AccountWithdrawService> logger)
    {

        public async Task<Contracts.ErrorOr<long>> ExecuteAsync(string accountNumber, decimal amount, CancellationToken cancellationToken)
        {
            try
            {
                var result = -1L;
                if (distributedLockMechanism is null)
                {
                    logger.LogCritical("Can not get the LockMechanism instance, {service}", nameof(AccountTransferService));
                    return new Contracts.InternalError("Can not perform the operation!");
                }
                await using (var @lock = await distributedLockMechanism.AcquireAsync($"lock:{accountNumber}", TimeSpan.FromSeconds(5)))
                {
                    if (!@lock.IsAcquired())
                        return new Contracts.LockError();

                    var account = await accountRepository.FirstOrDefaultAsync(account => account.AccountNumber == accountNumber);
                    if (account is null)
                        return new Contracts.ResourceNotFoundError(nameof(Account));



                    var withdrawResult = account.Withdraw(amount);
                    if (!withdrawResult.Success)
                        return withdrawResult.Errors;

                    await unitOfWork.BeginTransactionAsync(cancellationToken);
                    await accountRepository.Update(account);
                    await unitOfWork.CommitAsync(cancellationToken);

                    result = account.DomainEventResultOfType<AccountDebitCompleted, AccountDebitCompleteResult>()?.Transaction?.TrackingCode ?? -1;

                }
                return result;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                logger.LogError(ex, nameof(AccountWithdrawService));
                return ex;
            }
        }
    }
}
