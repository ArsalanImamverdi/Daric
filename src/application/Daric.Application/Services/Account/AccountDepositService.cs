using Daric.Domain.Accounts;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Accounts.Services;
using Daric.Domain.Shared;
using Daric.Domain.Transactions;
using Daric.Locking.Abstraction;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Services.Account
{
    public class AccountDepositService(IAccountRepository accountRepository,
                                       IUnitOfWork unitOfWork,
                                       IAccountDepositService accountDepositService,
                                       ITransactionRepository transactionRepository,
                                       IDistributedLockMechanism distributedLockMechanism,
                                       ILogger<AccountDepositService> logger)
    {
        public async Task<Contracts.ErrorOr<long>> ExecuteAsync(string accountNumber, decimal amount, CancellationToken cancellationToken)
        {
            try
            {
                long result = -1;
                if (distributedLockMechanism is null)
                {
                    logger.LogCritical("Can not get the LockMechanism instance, {service}", nameof(AccountTransferService));
                    return new Contracts.InternalError("Can not perform the operation!");
                }
                await using (var @lock = await distributedLockMechanism.AcquireAsync($"lock:{accountNumber}", TimeSpan.FromSeconds(5), cancellationToken: cancellationToken))
                {
                    if (!@lock.IsAcquired())
                        return new Contracts.LockError();

                    var account = await accountRepository.FirstOrDefaultAsync(account => account.AccountNumber == accountNumber);
                    if (account is null)
                        return new Contracts.ResourceNotFoundError(nameof(Account));

                    var depositResult = await accountDepositService.DepositAsync(account, amount, description: string.Empty, transactionRepository, cancellationToken);
                    if (!depositResult.Success)
                        return depositResult.Errors;

                    await unitOfWork.BeginTransactionAsync(cancellationToken);
                    await accountRepository.Update(account);

                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    await unitOfWork.CommitAsync(cancellationToken);

                    result = account.DomainEventResultOfType<AccountCreditCompleted, AccountCreditCompleteResult>()?.Transaction?.TrackingCode ?? -1;
                }

                return result;

            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                logger.LogError(ex, nameof(AccountDepositService));
                return ex;
            }
        }
    }
}
