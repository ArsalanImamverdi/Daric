using Daric.Domain.Accounts;
using Daric.Domain.Shared;
using Daric.Domain.Transactions;
using Daric.Locking.Abstraction;
using Daric.Scheduling.Abstraction;

using Microsoft.Extensions.Logging;

namespace Daric.Application.Jobs
{
    public class ScheduledTransferJob(ITransactionRepository transactionRepository,
                                      IAccountRepository accountRepository,
                                      IUnitOfWork unitOfWork,
                                      IDistributedLockMechanism distributedLockMechanism,
                                      ILogger<ScheduledTransferJob> logger) : IJob
    {
        public async Task ExecuteAsync(string name, IJobArguments arguments)
        {
            try
            {
                var transactions = await transactionRepository.GetPendingScheduledTransactionsAsync(CancellationToken.None);
                if (transactions.Length == 0)
                    return;

                foreach (var transaction in transactions)
                {
                    var account = await accountRepository.FirstOrDefaultAsync(account => account.Id == transaction.AccountId);
                    await using (var @lock = await distributedLockMechanism.TryAcquireAsync($"lock:{account!.AccountNumber}", TimeSpan.FromSeconds(5)))
                    {
                        if (!@lock.IsAcquired())
                            continue;
                        try
                        {
                            await unitOfWork.BeginTransactionAsync();

                            switch (transaction.TransactionType)
                            {
                                case TransactionType.Debit:
                                    account.Withdraw(transaction.Amount, performBy: PerformByType.System, currentTransaction: transaction);
                                    break;
                                case TransactionType.Credit:
                                    account.Deposit(transaction.Amount, performBy: PerformByType.System, currentTransaction: transaction);
                                    break;
                                default:
                                    break;
                            }
                            await unitOfWork.SaveChangesAsync();
                            await unitOfWork.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await unitOfWork.RollbackAsync();
                            logger.LogError(ex, "Transaction with Id:{id} has failed", transaction.Id);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(ScheduledTransferJob));
            }
        }
    }
}
