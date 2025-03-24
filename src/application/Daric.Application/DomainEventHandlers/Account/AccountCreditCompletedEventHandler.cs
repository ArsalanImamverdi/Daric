using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;
using Daric.Domain.Transactions;
using Daric.Domain.Transactions.DomainServices;

namespace Daric.Application.DomainEventHandlers.Account
{
    internal class AccountCreditCompletedEventHandler(ITransactionRepository transactionRepository,
                                                      ITrackingCodeGenerator trackingCodeGenerator) : DomainEventHandler<AccountCreditCompleted>
    {
        public override async Task<ErrorOr<bool>> HandleAsync(AccountCreditCompleted @event, CancellationToken cancellationToken)
        {
            try
            {
                if (@event.CurrentTransaction is not null)
                {
                    var transaction = (await transactionRepository.FirstOrDefaultAsync(transaction => transaction.Id == @event.CurrentTransaction.Id))!;
                    transaction.Done();
                    @event.Complete(new AccountCreditCompleteResult(transaction!));
                    await transactionRepository.Update(transaction);
                    return true;
                }
                {
                    var parentTransaction = ((AccountDebitCompleteResult)@event.AccountDebitCompleted?.Result!)?.Transaction;
                    var transaction = await Transaction.Create(@event.Account.Id, TransactionType.Credit, @event.Amount, @event.Description, @event.PerformBy, parentTransaction?.Id, @event.IsScheduled, trackingCodeGenerator);
                    if (!transaction)
                        return transaction.Errors;

                    await transactionRepository.Insert(transaction!);

                    @event.Complete(new AccountCreditCompleteResult(transaction!));
                    return true;
                }

            }
            catch (Exception ex)
            {
                @event.Fail(ex);
                return ex;
            }

        }
    }
}
