using Daric.Domain.Shared;
using Daric.Domain.Transactions.DomainEvents;
using Daric.Domain.Transactions.DomainServices;

using FluentAssertions;

using Moq;

namespace Daric.Domain.Transactions.Tests
{
    public class TransactionTests
    {
        private readonly Mock<ITrackingCodeGenerator> _trackingCodeGeneratorMock = new();

        [Fact]
        public async Task Create_ValidTransaction_ShouldSucceed()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var trackingCode = 123456789L;
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(trackingCode);

            // Act
            var result = await Transaction.Create(
                accountId: accountId,
                transactionType: TransactionType.Debit,
                amount: 1_000_000m,
                description: "Test transaction",
                performBy: PerformByType.User,
                parentId: null,
                isScheduled: false,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            );

            // Assert
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result.AccountId.Should().Be(accountId);
            result.Result.TrackingCode.Should().Be(trackingCode);
            result.Result.Amount.Should().Be(1_000_000m);
            result.Result.Status.Should().Be(TransactionStatus.Done);
            result.Result.PersistenceTransactionalEvents.Should()
                .ContainSingle(e => e is TransactionCreateTransactionalPersisted);
        }

        [Fact]
        public async Task Create_ZeroAmount_ShouldFail()
        {
            // Arrange
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(123456789L);

            // Act
            var result = await Transaction.Create(
                accountId: Guid.NewGuid(),
                transactionType: TransactionType.Debit,
                amount: 0m,
                description: "Invalid transaction",
                performBy: PerformByType.User,
                parentId: null,
                isScheduled: false,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            );

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e is NullOrEmptyError);
        }

        [Fact]
        public async Task Create_ScheduledTransaction_ShouldBePending()
        {
            // Arrange
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(123456789L);

            // Act
            var result = await Transaction.Create(
                accountId: Guid.NewGuid(),
                transactionType: TransactionType.Transfer,
                amount: 500_000m,
                description: "Scheduled transfer",
                performBy: PerformByType.System,
                parentId: null,
                isScheduled: true,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            );

            // Assert
            result.Result!.Status.Should().Be(TransactionStatus.Pending);
            result.Result.IsScheduled.Should().BeTrue();
        }

        [Fact]
        public async Task Done_ScheduledTransaction_ShouldMarkAsDone()
        {
            // Arrange
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(123456789L);

            var transaction = (await Transaction.Create(
                accountId: Guid.NewGuid(),
                transactionType: TransactionType.Bonus,
                amount: 2_000_000m,
                description: "Pending bonus",
                performBy: PerformByType.System,
                parentId: null,
                isScheduled: true,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            )).Result!;

            // Act
            var result = transaction.Done();

            // Assert
            result.Success.Should().BeTrue();
            transaction.Status.Should().Be(TransactionStatus.Done);
            transaction.IsScheduled.Should().BeFalse();
        }

        [Fact]
        public async Task Create_WithParentId_ShouldSetParentRelationship()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(123456789L);

            // Act
            var result = await Transaction.Create(
                accountId: Guid.NewGuid(),
                transactionType: TransactionType.Credit,
                amount: 750_000m,
                description: "Child transaction",
                performBy: PerformByType.User,
                parentId: parentId,
                isScheduled: false,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            );

            // Assert
            result.Result!.ParentId.Should().Be(parentId);
        }

        [Fact]
        public async Task Create_TrackingCodeGenerationFails_ShouldReturnError()
        {
            // Arrange
            _trackingCodeGeneratorMock
                .Setup(x => x.GetNextTrackingCodeAsync())
                .ReturnsAsync(new ErrorOr<long>([new InternalError("Tracking code service unavailable")]));

            // Act
            var result = await Transaction.Create(
                accountId: Guid.NewGuid(),
                transactionType: TransactionType.Debit,
                amount: 1_000_000m,
                description: "Failed transaction",
                performBy: PerformByType.User,
                parentId: null,
                isScheduled: false,
                trackingCodeGenerator: _trackingCodeGeneratorMock.Object
            );

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e is InternalError);
        }
    }
}
