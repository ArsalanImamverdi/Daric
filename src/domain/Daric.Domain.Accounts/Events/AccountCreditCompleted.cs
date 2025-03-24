using Daric.Domain.Shared;
using Daric.Domain.Transactions;

namespace Daric.Domain.Accounts.DomainEvents
{
    public record AccountCreditCompleteResult(Transaction Transaction) : IDomainEventResult;
    public record AccountCreditCompleted(Account Account,
                                         decimal Amount,
                                         string Description,
                                         PerformByType PerformBy,
                                         bool IsScheduled,
                                         Transaction? CurrentTransaction,
                                         AccountDebitCompleted? AccountDebitCompleted) : DomainEvent<AccountCreditCompleteResult>;
}
