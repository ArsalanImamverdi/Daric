using Daric.Domain.Shared;
using Daric.Domain.Transactions.DomainEvents;
using Daric.Domain.Transactions.DomainServices;

namespace Daric.Domain.Transactions
{
    public enum PerformByType
    {
        User,
        System
    }
    public enum TransactionType
    {
        Debit,
        Credit,
        Bonus,
        Transfer
    }
    public enum TransactionStatus
    {
        Pending,
        Done
    }

    public class Transaction : IAggregateRoot, IPersistenceTransactionalEventEntity
    {
        public Guid Id { get; private set; }
        public long TrackingCode { get; private set; }
        public Guid AccountId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public PerformByType PerformBy { get; private set; }
        public bool IsScheduled { get; private set; }
        public TransactionStatus Status { get; private set; }
        public List<IDomainEvent> PersistenceTransactionalEvents { get; }

        public Guid? ParentId { get; private set; }
        public Transaction? Parent { get; private set; }

        private Transaction()
        {
            PersistenceTransactionalEvents = [];
            Description = string.Empty;
        }
        private Transaction(long trackingCode, Guid accountId, decimal amount, DateTime createdAt, TransactionType transactionType, string description, PerformByType performBy, Guid? parentId, bool isScheduled)
        {
            TrackingCode = trackingCode;
            AccountId = accountId;
            Amount = amount;
            CreatedAt = createdAt;
            TransactionType = transactionType;
            Description = description;
            PerformBy = performBy;
            IsScheduled = isScheduled;
            Status = isScheduled ? TransactionStatus.Pending : TransactionStatus.Done;
            ParentId = parentId;
            PersistenceTransactionalEvents = [];
        }

        public ErrorOr<bool> Done()
        {
            IsScheduled = false;
            Status = TransactionStatus.Done;
            return true;
        }

        public static async Task<ErrorOr<Transaction>> Create(Guid accountId,
                                                              TransactionType transactionType,
                                                              decimal amount,
                                                              string description,
                                                              PerformByType performBy,
                                                              Guid? parentId,
                                                              bool isScheduled,
                                                              ITrackingCodeGenerator trackingCodeGenerator)
        {
            var errors = new List<Error>();

            if (accountId == Guid.Empty)
                errors.Add(new NullOrEmptyError(nameof(AccountId)));

            if (amount <= 0)
                errors.Add(new NullOrEmptyError(nameof(Amount)));

            if (string.IsNullOrWhiteSpace(description))
                errors.Add(new NullOrEmptyError(nameof(Description)));

            var trackingCode = await trackingCodeGenerator.GetNextTrackingCodeAsync();
            if (!trackingCode)
                errors.AddRange(trackingCode.Errors);

            if (errors.Count > 0)
                return errors;


            var transaction = new Transaction(trackingCode!, accountId, amount, DateTime.Now, transactionType, description, performBy, parentId, isScheduled);
            transaction.PersistenceTransactionalEvents.Add(new TransactionCreateTransactionalPersisted());
            return transaction;
        }


    }

}
