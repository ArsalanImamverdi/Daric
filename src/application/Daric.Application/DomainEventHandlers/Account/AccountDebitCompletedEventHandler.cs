using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;
using Daric.Domain.Transactions;
using Daric.Domain.Transactions.DomainServices;

namespace Daric.Application.DomainEventHandlers.Account
{
    internal class AccountDebitCompletedEventHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ITrackingCodeGenerator trackingCodeGenerator) : DomainEventHandler<AccountDebitCompleted>
    {
        public override async Task<ErrorOr<bool>> HandleAsync(AccountDebitCompleted @event, CancellationToken cancellationToken)
        {
            try
            {
                if (@event.CurrentTransaction is not null)
                {
                    var transaction = (await transactionRepository.FirstOrDefaultAsync(transaction => transaction.Id == @event.CurrentTransaction.Id))!;
                    transaction.Done();
                    await transactionRepository.Update(transaction);
                    return true;
                }
                {
                    var transaction = await Transaction.Create(@event.Account.Id, TransactionType.Debit, @event.Amount, @event.Description, @event.PerformBy, @event.ParentTransaction?.Id, false, trackingCodeGenerator);

                    if (!transaction)
                        return transaction.Errors;

                    @event.Complete(new AccountDebitCompleteResult(transaction!));
                    await transactionRepository.Insert(transaction!);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
