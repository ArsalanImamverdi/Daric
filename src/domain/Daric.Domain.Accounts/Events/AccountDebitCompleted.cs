using Daric.Domain.Shared;
using Daric.Domain.Transactions;

namespace Daric.Domain.Accounts.DomainEvents
{
    public record AccountDebitCompleteResult(Transaction Transaction) : IDomainEventResult;
    public record AccountDebitCompleted(Account Account,
                                        decimal Amount,
                                        string Description,
                                        PerformByType PerformBy,
                                        Transaction? CurrentTransaction,
                                        Transaction? ParentTransaction) : DomainEvent<AccountDebitCompleteResult>
    {
    }
}
