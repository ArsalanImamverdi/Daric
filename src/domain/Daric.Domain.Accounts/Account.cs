using Daric.Domain.Accounts.DomainErrors;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;
using Daric.Domain.Transactions;

namespace Daric.Domain.Accounts
{
    public sealed class Account : IAggregateRoot, IPersistenceTransactionalEventEntity
    {
        public Guid Id { get; private set; }
        public string AccountNumber { get; private set; }
        public Guid CustomerId { get; private set; }
        public decimal Balance { get; private set; }

        List<IDomainEvent> IPersistenceTransactionalEventEntity.PersistenceTransactionalEvents { get; } = [];


        private Account()
        {
            AccountNumber = string.Empty;
            CustomerId = Guid.Empty;
            Balance = 0;
        }
        private Account(string accountNumber, Guid customerId)
        {
            AccountNumber = accountNumber;
            CustomerId = customerId;
        }
        public static async Task<ErrorOr<Account>> Create(Guid customerId, IAccountNumberGenerator accountNumberGenerator)
        {
            var errors = new List<Error>();
            if (customerId == Guid.Empty)
            {
                errors.Add(new NullOrEmptyError(nameof(CustomerId)));
            }

            var accountNumber = await accountNumberGenerator.GetNextAccountNumberAsync();
            if (!accountNumber)
                errors.AddRange(accountNumber.Errors);

            if (errors.Count > 0)
                return errors;

            var account = new Account(accountNumber!, customerId);
            account.AddPersistenceTransactionalEvent(new AccountCreatePersisted(accountNumber!, customerId));
            return account;
        }

        public ErrorOr<bool> Deposit(decimal amount,
                                     string description = "",
                                     PerformByType performBy = PerformByType.User,
                                     bool scheduledTransaction = false,
                                     Transaction? currentTransaction = null,
                                     AccountDebitCompleted? debitCompletedEvent = null)
        {
            if (!scheduledTransaction)
                Balance += amount;

            if (string.IsNullOrWhiteSpace(description))
                description = nameof(Deposit);

            AddPersistenceTransactionalEvent(new AccountCreditCompleted(this, amount, description, performBy, scheduledTransaction, currentTransaction, debitCompletedEvent));
            if (!scheduledTransaction)
                AddPersistenceTransactionalEvent(new AccountBalanceUpdated(this));
            return true;
        }

        public ErrorOr<bool> Withdraw(decimal amount, string description = "", PerformByType performBy = PerformByType.User, Transaction? currentTransaction = null, Transaction? parentTransaction = null)
        {
            if (Balance < amount)
                return new NotEnoughBalanceError();

            Balance -= amount;

            if (string.IsNullOrWhiteSpace(description))
                description = nameof(Withdraw);

            if (string.IsNullOrWhiteSpace(description))
                description = nameof(Withdraw);

            AddPersistenceTransactionalEvent(new AccountDebitCompleted(this, amount, description, performBy, currentTransaction, parentTransaction));
            AddPersistenceTransactionalEvent(new AccountBalanceUpdated(this));

            return true;
        }

        public ErrorOr<bool> Bonus(decimal amount, string reason)
        {
            return Deposit(amount, string.Join(' ', reason, nameof(Bonus)), PerformByType.System);
        }
        private void AddPersistenceTransactionalEvent(IDomainEvent domainEvent)
        {
            (this as IPersistenceTransactionalEventEntity).PersistenceTransactionalEvents.Add(domainEvent);
        }
        public TEvent? DomainEventOfType<TEvent>() where TEvent : class, IDomainEvent
        {
            return (this as IPersistenceTransactionalEventEntity).PersistenceTransactionalEvents.FirstOrDefault(@event => @event is TEvent) as TEvent;
        }

        public TEventResult? DomainEventResultOfType<TEvent, TEventResult>() where TEvent : class, IDomainEvent where TEventResult : class, IDomainEventResult
        {
            var @event = (this as IPersistenceTransactionalEventEntity).PersistenceTransactionalEvents.FirstOrDefault(@event => @event is TEvent);
            return (TEventResult)@event?.Result!;
        }
    }
}
