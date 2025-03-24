using Daric.Domain.Accounts;
using Daric.Domain.Accounts.DomainErrors;
using Daric.Domain.Accounts.DomainEvents;
using Daric.Domain.Shared;

using FluentAssertions;

using Moq;

namespace Daric.Domain.Account.Tests
{
    public class AccountTests
    {
        [Fact]
        public async Task Create_ValidAccount_ShouldSucceed()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var result = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.True(result);
            // Assert
            result.Success.Should().BeTrue();
            result.Result!.AccountNumber.Should().Be("ACC123456");
            result.Result.CustomerId.Should().Be(customerId);
            result.Result.Balance.Should().Be(0);
            result.Result.DomainEventOfType<AccountCreatePersisted>().Should().NotBeNull();
        }

        [Fact]
        public async Task Create_EmptyCustomerId_ShouldFail()
        {
            // Arrange
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");
            // Act
            var result = await Accounts.Account.Create(Guid.Empty, mockGenerator.Object);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e is NullOrEmptyError);
        }

        [Fact]
        public async Task Deposit_ValidAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var account = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.NotNull(account);
            var initialBalance = account.Result!.Balance;

            var result = account.Result!.Deposit(1000m, "Salary");
            // Assert



            result.Success.Should().BeTrue();
            account.Result!.Balance.Should().Be(initialBalance + 1000m);
            account.Result.DomainEventOfType<AccountCreditCompleted>().Should().NotBeNull();
            account.Result.DomainEventOfType<AccountBalanceUpdated>().Should().NotBeNull();
        }

        [Fact]
        public async Task Withdraw_SufficientBalance_ShouldDecreaseBalance()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var account = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.NotNull(account);
            account.Success.Should().BeTrue();

            account.Result!.Deposit(1000m, "Salary");
            var result = account.Result.Withdraw(500m, "Rent");

            // Assert
            result.Success.Should().BeTrue();
            account.Result.Balance.Should().Be(500m);
            account.Result.DomainEventOfType<AccountDebitCompleted>().Should().NotBeNull();
        }


        [Fact]
        public async Task Withdraw_InsufficientBalance_ShouldFail()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var account = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.NotNull(account);
            account.Success.Should().BeTrue();

            var result = account.Result!.Withdraw(500m, "Rent");

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e is NotEnoughBalanceError);

        }


        [Fact]
        public async Task Bonus_ValidAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var account = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.NotNull(account);
            account.Success.Should().BeTrue();

            var result = account.Result!.Bonus(500m, "Referral");

            // Assert
            result.Success.Should().BeTrue();
            account.Result.Balance.Should().Be(500m);
            account.Result.DomainEventOfType<AccountCreditCompleted>().Should().NotBeNull();
        }

        [Fact]
        public async Task Deposit_Scheduled_ShouldNotUpdateBalance()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var account = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.NotNull(account);
            account.Success.Should().BeTrue();

            var result = account.Result!.Deposit(1000m, scheduledTransaction: true);

            // Assert
            result.Success.Should().BeTrue();
            account.Result.Balance.Should().Be(0); // Balance unchanged
            account.Result.DomainEventOfType<AccountBalanceUpdated>().Should().BeNull();
        }

        [Fact]
        public async Task DomainEventOfType_ShouldReturnCorrectEvent()
        { // Arrange
            var customerId = Guid.NewGuid();
            var mockGenerator = new Mock<IAccountNumberGenerator>();
            mockGenerator.Setup(x => x.GetNextAccountNumberAsync())
                        .ReturnsAsync("ACC123456");

            // Act
            var account = await Accounts.Account.Create(customerId, mockGenerator.Object);

            Assert.NotNull(account);
            account.Success.Should().BeTrue();
            account.Result!.Deposit(100m, "Salary");

            var creditEvent = account.Result!.DomainEventOfType<AccountCreditCompleted>();
            var debitEvent = account.Result.DomainEventOfType<AccountDebitCompleted>();

            // Assert
            creditEvent.Should().NotBeNull();
            debitEvent.Should().BeNull();
        }
    }
}