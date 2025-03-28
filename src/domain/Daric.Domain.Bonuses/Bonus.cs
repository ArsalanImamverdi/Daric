using Daric.Domain.Bonuses.Events;
using Daric.Domain.Shared;

namespace Daric.Domain.Bonuses
{
    public enum BonusType
    {
        WelcomePack,
        Referral
    }
    public sealed class Bonus : IAggregateRoot, IPersistenceTransactionalEventEntity
    {
        public Guid Id { get; private set; }
        public BonusType Type { get; private set; }
        public Guid AccountId { get; private set; }
        public decimal Amount { get; private set; }
        public List<IDomainEvent> PersistenceTransactionalEvents { get; }

        private Bonus()
        {
            PersistenceTransactionalEvents = [];
        }

        private Bonus(Guid accountId, BonusType bonusType, decimal amount) : this()
        {
            AccountId = accountId;
            Type = bonusType;
            Amount = amount;

        }
        public static ErrorOr<Bonus> Create(Guid accountId, BonusType bonusType, decimal amount)
        {
            var errors = new List<Error>();

            if (accountId == Guid.Empty)
                errors.Add(new NullOrEmptyError(nameof(AccountId)));

            if (amount <= 0)
                errors.Add(new InvalidDataError(nameof(Amount)));

            if (errors.Count > 0)
                return errors;

            var bonus = new Bonus(accountId, bonusType, amount); ;
            bonus.PersistenceTransactionalEvents.Add(new BonusCreatePersisted(accountId, bonusType.ToString(), amount));
            return bonus;
        }

        public static ErrorOr<Bonus> CreateWelcomePack(Guid accountId)
        {
            return Create(accountId, BonusType.WelcomePack, 2_000_000);
        }
    }
}
