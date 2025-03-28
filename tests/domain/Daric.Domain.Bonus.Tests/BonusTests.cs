using Daric.Domain.Bonuses;
using Daric.Domain.Bonuses.Events;
using Daric.Domain.Shared;

using FluentAssertions;

namespace Daric.Domain.Bonus.Tests
{
    public class BonusTests
    {
        [Fact]
        public void Create_ValidBonus_ShouldSucceed()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var bonusType = BonusType.Referral;
            var amount = 1_000_000m;

            // Act
            var result = Bonuses.Bonus.Create(accountId, bonusType, amount);

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result.AccountId.Should().Be(accountId);
            result.Result.Type.Should().Be(bonusType);
            result.Result.Amount.Should().Be(amount);
            result.Result.PersistenceTransactionalEvents.Should()
                .ContainSingle(e => e is BonusCreatePersisted);
        }

        [Fact]
        public void Create_ZeroAmount_ShouldFail()
        {
            // Act
            var result = Bonuses.Bonus.Create(Guid.NewGuid(), BonusType.Referral, 0m);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e is InvalidDataError);
        }

        [Fact]
        public void CreateWelcomePack_ShouldUseFixedAmount()
        {
            // Act
            var result = Bonuses.Bonus.CreateWelcomePack(Guid.NewGuid());

            // Assert
            result.Success.Should().BeTrue();
            result.Result!.Type.Should().Be(BonusType.WelcomePack);
            result.Result.Amount.Should().Be(2_000_000m);
        }

        [Fact]
        public void Create_ShouldRaiseBonusCreatePersistedEvent()
        {
            // Act
            var result = Bonuses.Bonus.Create(Guid.NewGuid(), BonusType.Referral, 500_000m);

            // Assert
            var @event = result.Result!.PersistenceTransactionalEvents.First();
            @event.Should().BeOfType<BonusCreatePersisted>();
            var persistedEvent = (BonusCreatePersisted)@event;
            persistedEvent.AccountId.Should().Be(result.Result.AccountId);
            persistedEvent.Description.Should().Be(result.Result.Type.ToString());
            persistedEvent.Amount.Should().Be(result.Result.Amount);
        }

        [Fact]
        public void Bonus_ShouldBeImmutable()
        {
            // Arrange
            var bonus = Bonuses.Bonus.Create(Guid.NewGuid(), BonusType.Referral, 1_000_000m).Result;

            // Assert
            typeof(Bonuses.Bonus).GetProperty(nameof(Bonuses.Bonus.AccountId))!.SetMethod!
        .IsPrivate.Should().BeTrue("AccountId should have private setter");

            typeof(Bonuses.Bonus).GetProperty(nameof(Bonuses.Bonus.Type))!.SetMethod!
                .IsPrivate.Should().BeTrue("Type should have private setter");

            typeof(Bonuses.Bonus).GetProperty(nameof(Bonuses.Bonus.Amount))!.SetMethod!
                .IsPrivate.Should().BeTrue("Amount should have private setter");
        }

        [Fact]
        public void Create_EmptyAccountId_ShouldFail()
        {
            var result = Bonuses.Bonus.Create(Guid.Empty, BonusType.Referral, 1_000_000m);
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(e => e is NullOrEmptyError);
        }
    }
}
